using LibraryManagement.Contracts;       // CreateBookDto, BookDto, UpdateBookDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibraryManagement.UI.Pages.Admin.Books
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient _client;
        public CreateModel(IHttpClientFactory httpFactory)
            => _client = httpFactory.CreateClient("ApiClient");

        [BindProperty]
        public CreateBookDto Book { get; set; } = new();

        // Lista autorów do dropdowna
        public List<AuthorDto> Authors { get; set; } = new();

        public async Task OnGetAsync()
        {
            // pobierz autorów z API
            Authors = await _client.GetFromJsonAsync<List<AuthorDto>>("api/authors")
                      ?? new List<AuthorDto>();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();  // za³aduj ponownie Authors
                return Page();
            }

            var response = await _client.PostAsJsonAsync("api/books", Book);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            ModelState.AddModelError("", "Nie uda³o siê dodaæ ksi¹¿ki.");
            await OnGetAsync();
            return Page();
        }
    }
}
