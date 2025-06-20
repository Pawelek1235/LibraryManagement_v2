using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Books
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _client;
        public EditModel(IHttpClientFactory httpFactory)
            => _client = httpFactory.CreateClient("ApiClient");

        [BindProperty]
        public UpdateBookDto Book { get; set; } = new();

        // <<< dodajemy list� autor�w
        public List<AuthorDto> Authors { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // pobierz list� autor�w
            Authors = await _client.GetFromJsonAsync<List<AuthorDto>>("api/authors")
                      ?? new List<AuthorDto>();

            // pobierz dane ksi��ki
            var dto = await _client.GetFromJsonAsync<BookDto>($"api/books/{id}");
            if (dto == null) return NotFound();

            Book = new UpdateBookDto
            {
                Id = dto.Id,
                Title = dto.Title,
                Isbn = dto.Isbn,
                AuthorId = dto.AuthorId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // je�li walidacja nie przejdzie, musimy ponownie za�adowa� autor�w
                Authors = await _client.GetFromJsonAsync<List<AuthorDto>>("api/authors")
                          ?? new List<AuthorDto>();
                return Page();
            }

            var response = await _client.PutAsJsonAsync($"api/books/{Book.Id}", Book);
            if (response.IsSuccessStatusCode)
                return RedirectToPage("./Index");

            ModelState.AddModelError("", "Nie uda�o si� zapisa� zmian.");
            // i zn�w autorzy
            Authors = await _client.GetFromJsonAsync<List<AuthorDto>>("api/authors")
                      ?? new List<AuthorDto>();
            return Page();
        }
    }
}
