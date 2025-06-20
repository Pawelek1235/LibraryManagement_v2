using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Services;
using Microsoft.Extensions.Options;
using Xunit;

namespace LibraryManagement.Tests
{
    public class JwtTokenGeneratorTests
    {
        private JwtTokenGenerator CreateGenerator(int expiresMinutes = 30)
        {
            var settings = new AppSettings
            {
                JwtSecret = "TestSecretKey1234567890",
                JwtIssuer = "UnitTestIssuer",
                JwtAudience = "UnitTestAudience",
                JwtExpiresMinutes = expiresMinutes
            };
            var options = Options.Create(settings);
            return new JwtTokenGenerator(options);
        }

        [Fact]
        public void GenerateToken_Should_Contain_Correct_Issuer_Audience_And_Claims()
        {
            // Arrange
            var gen = CreateGenerator(expiresMinutes: 15);
            var userId = 99;
            var role = "Admin";

            // Act
            var tokenString = gen.GenerateToken(userId, role);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(tokenString);

            // Assert issuer/audience
            Assert.Equal("UnitTestIssuer", jwt.Issuer);
            Assert.Equal("UnitTestAudience", jwt.Audiences.Single());

            // Assert claims
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == userId.ToString());
            Assert.Contains(jwt.Claims, c => c.Type == ClaimTypes.Role && c.Value == role);

            // Assert expiration (~15 minutes ahead)
            var minRemaining = (jwt.ValidTo - DateTime.UtcNow).TotalMinutes;
            Assert.InRange(minRemaining, 14, 16);
        }
    }
}
