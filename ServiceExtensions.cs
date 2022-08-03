using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using AspNetCoreRateLimit;
using QulixTest.Persistence;
using QulixTest.Core.Domain;
using QulixTest.Core;

namespace QulixTest;


public static class ServiceExtensions
{
    public static void ConfigureIdentity(this IServiceCollection services)
    {
        /// to configure database 
        var builder = services.AddIdentityCore<Author>(q => q.User.RequireUniqueEmail = true);

        builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);

        builder.AddEntityFrameworkStores<DatabaseDbContext>().AddDefaultTokenProviders();
    }

    /// configuring the jwt web token
    public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
    {
        var jwtSettings = Configuration.GetSection("Jwt");

        var key = Environment.GetEnvironmentVariable("KEY");

        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
          .AddJwtBearer(o =>
          {
              o.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  //ValidIssuer = jwtSettings.GetSection("Issuer").Value,
                  ValidAudience = Configuration["Jwt: ValidAudience"],
                  ValidIssuer = Configuration["Jwt:ValidIssuer"],
                  IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
              };
          });
    }

    /// to catch exceptions and write to the log file
    public static void ConfigureExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(error =>
        {
            error.Run(async context =>
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";
                var contexFeature = context.Features.Get<IExceptionHandlerFeature>();

                if (contexFeature != null)
                {
                    Log.Error($"Something went wrong in the {contexFeature.Error}");

                    await context.Response.WriteAsync(new Error
                    {

                        StatusCode = context.Response.StatusCode,
                        Message = "Internal Server Error. Please try again later"
                    }.ToString());
                }
            });
        });
    }
    /// If new data is added user must not get the old one
    ///  from the cache in his/her browser
    /// instead configure the cache that when new is added refresh the cache
    public static void ConfigureHttpCacheHeader(this IServiceCollection service)
    {
        // Globally declared caching config
        service.AddResponseCaching();
        service.AddHttpCacheHeaders(
            (expirationOpt) =>
            {
                expirationOpt.MaxAge = 150;
                expirationOpt.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
            },
            (validationOpt) =>
            {
                validationOpt.MustRevalidate = true;
            }
            );
    }

    // Configuration of the rate limit
    public static void ConfigureRateLimiting(this IServiceCollection service)
    {
        var rateLimitRules = new List<RateLimitRule>
        {
            new RateLimitRule
            {
                Endpoint = "*",
                Limit = 10,
                Period = "5s"
            }
        };
        service.Configure<IpRateLimitOptions>(option =>
        {
            option.GeneralRules = rateLimitRules;
        });
        service.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
        service.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        service.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
    }
}

