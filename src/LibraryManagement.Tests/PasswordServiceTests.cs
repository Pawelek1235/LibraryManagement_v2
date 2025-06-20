using LibraryManagement.Infrastructure.Services;
using Xunit;

namespace LibraryManagement.Tests
{
    public class PasswordServiceTests
    {
        [Fact]
        public void CreateHash_And_Verify_Works()
        {
            // Arrange
            var pwd = "P@ssw0rd!";

            // Act
            PasswordService.CreatePasswordHash(pwd, out var hash, out var salt);

            // zamiast VerifyPasswordHash ­– w Twoim serwisie używasz VerifyPassword:
            var isValid = PasswordService.VerifyPassword(pwd, hash, salt);
            var isWrong = PasswordService.VerifyPassword("wrong", hash, salt);

            // Assert
            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.True(isValid, "Poprawne hasło powinno się weryfikować");
            Assert.False(isWrong, "Niepoprawne hasło nie powinno się weryfikować");
        }
    }
}
