using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using LibraryManagement.Contracts;             // LoginRequestDto, AuthResponseDto
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _client;

        public LoginModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawid³owy format email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Has³o jest wymagane")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            ErrorMessage = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1) Przygotuj DTO logowania
            var loginReq = new LoginRequestDto
            {
                Email = Email,
                Password = Password
            };

            // 2) Wyœlij do API
            var response = await _client.PostAsJsonAsync("api/auth/login", loginReq);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Niepoprawny email lub has³o";
                return Page();
            }

            // 3) Odczytaj odpowiedŸ z serwera
            var auth = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (auth is null)
            {
                ErrorMessage = "B³¹d podczas logowania";
                return Page();
            }

            // 4) Przygotuj claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
                new Claim(ClaimTypes.Name,           Email),
                new Claim(ClaimTypes.Role,           auth.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 5) Oblicz moment wygaœniêcia cookie
            var expiresUtc = DateTimeOffset.UtcNow.AddMinutes(auth.ExpiresInMinutes);

            // 6) Zaloguj u¿ytkownika (cookie)
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = expiresUtc
                });

            // 7) Przekieruj do panelu admin
            return RedirectToPage("/Admin/Books/Index");
        }
    }
}
