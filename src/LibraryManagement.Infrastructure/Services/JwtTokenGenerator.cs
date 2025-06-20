using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LibraryManagement.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LibraryManagement.Infrastructure.Services
{
    public class JwtTokenGenerator
    {
        private readonly AppSettings _settings;

        public JwtTokenGenerator(IOptions<AppSettings> options)
        {
            _settings = options.Value;
        }

    
        public int ExpiresInMinutes => _settings.JwtExpiresMinutes;

 
        public string GenerateToken(int userId, string role)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_settings.JwtSecret);
            var signingKey = new SymmetricSecurityKey(keyBytes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(ClaimTypes.Role,           role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_settings.JwtExpiresMinutes),
                Issuer = _settings.JwtIssuer,
                Audience = _settings.JwtAudience,
                SigningCredentials = new SigningCredentials(
                    signingKey,
                    SecurityAlgorithms.HmacSha256)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(tokenDescriptor);
            return handler.WriteToken(token);
        }
    }
}
