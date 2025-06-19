using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using LibraryManagement.UI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Books
{
    public class EditModel : PageModel
    {
        private readonly HttpClient _client;

        public EditModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        public Book Book { get; set; }

        /// <summary>
        /// Lista autor�w do dropdowna
        /// </summary>
        public List<AuthorDto> Authors { get; set; } = new();

        public async Task OnGetAsync(int? id)
        {
            // Dodajemy JWT
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {Request.Cookies["jwt"]}");

            // Pobranie wszystkich autor�w
            Authors = await _client.GetFromJsonAsync<List<AuthorDto>>("api/authors")
                      ?? new List<AuthorDto>();

            // Nowy lub istniej�cy obiekt Book
            if (id.HasValue && id.Value > 0)
            {
                Book = await _client.GetFromJsonAsync<Book>($"api/books/{id.Value}")
                       ?? new Book();
            }
            else
            {
                Book = new Book();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Od�wie� token
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add(
                "Authorization", $"Bearer {Request.Cookies["jwt"]}");

            // Uzupe�nij wymagane pola, by API nie rzuca�o 400:
            Book.Loans ??= new List<Loan>();

            // Je�eli API wymaga obiektu Author zamiast tylko AuthorId:
            var selectedAuthor = Authors.FirstOrDefault(a => a.Id == Book.AuthorId);
            if (selectedAuthor != null)
            {
                Book.Author = new Author
                {
                    Id = selectedAuthor.Id,
                    FullName = selectedAuthor.FullName
                    // wype�nij inne pola Author, je�li potrzebne
                };
            }

            if (Book.Id == 0)
            {
                // Create
                var response = await _client.PostAsJsonAsync("api/books", Book);
                response.EnsureSuccessStatusCode();
            }
            else
            {
                // Update
                var response = await _client.PutAsJsonAsync($"api/books/{Book.Id}", Book);
                response.EnsureSuccessStatusCode();
            }

            return RedirectToPage("Index");
        }
    }
}
