using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Contracts;    // BookDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages
{
    public class BookSearchModel : PageModel
    {
        private readonly HttpClient _client;

        // U¿ywamy ApiClient, bo w Program.cs jest on skonfigurowany z BaseAddress do Twojego API
        public BookSearchModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        public string Query { get; set; }

        public List<BookDto> Results { get; set; } = new();

        public void OnGet()
        {
            // tylko pokazujemy stronê – bez wyszukiwania
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                ModelState.AddModelError(nameof(Query), "Podaj tytu³ do wyszukania.");
                return Page();
            }

            try
            {
                // Skonstruuj œcie¿kê wzglêdn¹ – BaseAddress jest ju¿ ustawione w ApiClient
                var url = $"api/books/search?title={Uri.EscapeDataString(Query)}";

                // Wyœlij ¿¹danie GET i deserializuj JSON do List<BookDto>
                var response = await _client.GetFromJsonAsync<List<BookDto>>(url);
                Results = response ?? new List<BookDto>();
            }
            catch (HttpRequestException httpEx)
            {
                ModelState.AddModelError("", $"B³¹d HTTP: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Nieoczekiwany b³¹d: {ex.Message}");
            }

            return Page();
        }
    }
}
