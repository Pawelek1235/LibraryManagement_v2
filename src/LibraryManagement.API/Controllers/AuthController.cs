using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagement.API.Models;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Services;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            // 1. Znajdź użytkownika po email
            var members = await _uow.Members.GetAllAsync();
            var member = members
                          .FirstOrDefault(m =>
                              m.Email.Equals(req.Email, StringComparison.OrdinalIgnoreCase));

            if (member == null)
                return Unauthorized(new { message = "Nieprawidłowy email lub hasło." });

            // 2. Zweryfikuj hasło
            if (!PasswordService.VerifyPassword(req.Password,
                                                member.PasswordHash,
                                                member.PasswordSalt))
            {
                return Unauthorized(new { message = "Nieprawidłowy email lub hasło." });
            }

            // 3. Wygeneruj token i oblicz datę wygaśnięcia
            var token = _jwtGen.GenerateToken(member.Id, member.Role);
            var expires = DateTime.UtcNow.AddMinutes(_jwtGen.ExpiresMinutes);

            // 4. Zwróć wynik
            var response = new AuthResponse
            {
                Token = token,
                Expires = expires
            };
            return Ok(new AuthResponse
            {
                Token = token,
                Expires = expires,
                UserId = member.Id,
                Role = member.Role      // zakładam, że masz Member.Role = "Admin"
            });

        }
    }
}
