using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using QulixTest.Core.IRepositories;
using QulixTest.Persistence.Repositories;
using QulixTest.Persistence.AuthServive;
using QulixTest.Core.Domain;
using QulixTest.Persistence;
using QulixTest.Persistence.MapperConfigurations;
using QulixTest;
using AspNetCoreRateLimit;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.
            AllowAnyMethod()
            .AllowAnyHeader();
        });
});

///<summary>
/// Configure the Serilog Logger
/// </summary>
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(MapperInitilizer));

builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuthManager, AuthManager>();
builder.Services.AddIdentity<Author, IdentityRole>()
    .AddEntityFrameworkStores<DatabaseDbContext>()
    .AddDefaultTokenProviders();

/// declare global cahshing in controllers using program.cs
builder.Services.AddControllers(config =>
{
    config.CacheProfiles.Add("SecondsDuration", new Microsoft.AspNetCore.Mvc.CacheProfile
    {
        Duration = 120
    });
}).AddNewtonsoftJson(op =>
    op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );

builder.Services.AddDbContext<DatabaseDbContext>(options =>

    options.UseSqlServer(builder.Configuration.GetConnectionString("NewConnection"))

);
//// Coming from the service extensions
builder.Services.ConfigureHttpCacheHeader();

builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(Configuration);

// add memory cashe while rate limit uses it
builder.Services.AddMemoryCache();

/// our method and one moreto get access to actual controller
builder.Services.ConfigureRateLimiting();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
 options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

/// register cashing 
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ConfigureExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod()
                            .SetIsOriginAllowed(_ => true).AllowCredentials());


app.UseResponseCaching();

app.UseHttpCacheHeaders();

app.UseRouting();

app.UseCors(MyAllowSpecificOrigins);////Cors

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
