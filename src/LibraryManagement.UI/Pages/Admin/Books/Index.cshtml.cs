using LibraryManagement.Core.Entities;
using LibraryManagement.Grpc.Protos;
using LibraryManagement.UI.Models;        // ? dodaj tê przestrzeñ nazw
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace LibraryManagement.UI.Pages.Admin.Books
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;
        public IndexModel(IHttpClientFactory httpFactory)
            => _client = httpFactory.CreateClient("ApiClient");

        // zamiast IEnumerable<Book> u¿yj List<Book>
        public List<BookDto> Books { get; set; } = new();

        public async Task OnGetAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            // GetFromJsonAsync zwraca List<Book>, wiêc od razu przypisujesz
            Books = await _client.GetFromJsonAsync<List<BookDto>>("api/books")
           ?? new List<BookDto>();
        }
    }

}
