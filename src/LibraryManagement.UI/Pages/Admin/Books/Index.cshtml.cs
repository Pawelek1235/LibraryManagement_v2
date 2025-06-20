using LibraryManagement.UI.Models; // BookDto
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LibraryManagement.Grpc.Protos;
namespace LibraryManagement.UI.Pages.Admin.Books
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _client;

        public IndexModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        public List<BookDto> Books { get; set; } = new();

        public async Task OnGetAsync()
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            Books = await _client.GetFromJsonAsync<List<BookDto>>("api/books")
                    ?? new List<BookDto>();
        }

        // handler wywo�ywany przez <form asp-page-handler="Delete">
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            var response = await _client.DeleteAsync($"api/books/{id}");
            if (response.IsSuccessStatusCode)
            {
                // mo�esz od�wie�y� list�, albo po prostu przekierowa� i prze�adowa� OnGet
                return RedirectToPage();
            }
            ModelState.AddModelError("", "B��d usuwania ksi��ki.");
            await OnGetAsync();
            return Page();
        }
    }
}
