using LibraryManagement.Infrastructure.Services;
using Xunit;

namespace LibraryManagement.Tests
{
    public class PasswordServiceTests
    {
        [Fact]
        public void CreateHash_And_Verify_Works()
        {
    
            var pwd = "P@ssw0rd!";

            PasswordService.CreatePasswordHash(pwd, out var hash, out var salt);
         
            var isValid = PasswordService.VerifyPassword(pwd, hash, salt);
            var isWrong = PasswordService.VerifyPassword("wrong", hash, salt);

            Assert.NotNull(hash);
            Assert.NotNull(salt);
            Assert.True(isValid, "Poprawne hasło powinno się weryfikować");
            Assert.False(isWrong, "Niepoprawne hasło nie powinno się weryfikować");
        }
    }
}
