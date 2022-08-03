using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using QulixTest.Core.Domain;
using QulixTest.Core.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace QulixTest.Persistence.AuthServive
{
    public class AuthManager : IAuthManager
    {
        private readonly UserManager<Author> _userManager;
        private readonly IConfiguration _configuration;
        private Author _user;

        public AuthManager(UserManager<Author> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<string> CreateToken()
        {
            var signingCredentials = GetSigningCredentials();
            var claims = await GetClaims();
            var token = GenerateTokenOptions(signingCredentials, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("Jwt");

            //int expiryMins = 10;
            var exp = DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.GetSection("ValidIssuer").Value,
                claims: claims,
                //expires: DateTime.UtcNow.AddMinutes(expiryMins),
                expires: exp,
                signingCredentials: signingCredentials
                );
            return token;
        }

        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var key = Environment.GetEnvironmentVariable("KEY");
            var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }

        public async Task<bool> ValidateUser(LoginUserDTO userDTO)
        {
            _user = await _userManager.FindByNameAsync(userDTO.Email);
            return (_user != null && await _userManager.CheckPasswordAsync(_user, userDTO.Password));
        }
    }
}
