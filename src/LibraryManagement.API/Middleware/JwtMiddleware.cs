using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibraryManagement.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LibraryManagement.Infrastructure.Configuration;


namespace LibraryManagement.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _secret;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> options)
        {
            _next = next;
            _secret = options.Value.JwtSecret;
        }

        public async Task Invoke(HttpContext context, IUnitOfWork uow)
        {
            var token = context.Request.Headers["Authorization"]
                                .FirstOrDefault()?
                                .Split(" ")
                                .Last();

            if (!string.IsNullOrEmpty(token))
                AttachUserToContext(context, token);

            await _next(context);
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_secret);

                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwt = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var role = jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value;

            

                var claimsIdentity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                    new Claim(ClaimTypes.Role, role)
                }, "jwt");

                context.User = new ClaimsPrincipal(claimsIdentity);
            }
            catch
            {
              
            }
        }
    }
}
