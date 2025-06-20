using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using LibraryManagement.Contracts;


namespace LibraryManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        private readonly JwtTokenGenerator _jwtGen;

        public AuthController(IUnitOfWork uow, JwtTokenGenerator jwtGen)
        {
            _uow = uow;
            _jwtGen = jwtGen;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto req)
        {
            var members = await _uow.Members.GetAllAsync();
            var member = members
                .FirstOrDefault(m =>
                    m.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase));

            if (member == null ||
                !PasswordService.VerifyPassword(req.Password,
                                               member.PasswordHash,
                                               member.PasswordSalt))
            {
                return Unauthorized(new { message = "Nieprawidłowy email lub hasło." });
            }

            var token = _jwtGen.GenerateToken(member.Id, member.Role);
            var expires = DateTime.UtcNow.AddMinutes(_jwtGen.ExpiresInMinutes);

            var response = new AuthResponseDto
            {
                Token = token,
                ExpiresInMinutes = _jwtGen.ExpiresInMinutes,
                UserId = member.Id,
                Role = member.Role
            };

            return Ok(response);
        }
    }
}
