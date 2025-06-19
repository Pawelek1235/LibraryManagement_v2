using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using LibraryManagement.API.Models;
using LibraryManagement.UI.Models;                 // ? import Twoich modeli
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient _client;

        // Wstrzykujemy nazwany HttpClient "ApiClient" skonfigurowany w Program.cs
        public LoginModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress(ErrorMessage = "Nieprawid≥owy format email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Has≥o jest wymagane")]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
            // Czyúcimy poprzednie komunikaty
            ErrorMessage = null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // 1) Przygotuj øπdanie logowania
            var loginReq = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            // 2) Wyúlij do API (relatywne URI, bo BaseAddress ustawiony)
            var response = await _client.PostAsJsonAsync("api/auth/login", loginReq);
            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Niepoprawny email lub has≥o";
                return Page();
            }

            // 3) Odczytaj odpowiedü z tokenem, expires i rolπ
            var auth = await response.Content.ReadFromJsonAsync<AuthResponse>();

            // 4) Przygotuj claims (UserId ? string)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
                new Claim(ClaimTypes.Name, Email),
                new Claim(ClaimTypes.Role, auth.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // 5) Zaloguj i ustaw cookie na okres waønoúci tokenu
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = auth.Expires
                });

            // 6) Przekieruj do panelu Admin
            return RedirectToPage("/Admin/Books/Index");
        }
    }
}
