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

      
        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            _client.DefaultRequestHeaders.Remove("Authorization");
            _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            var response = await _client.DeleteAsync($"api/books/{id}");
            if (response.IsSuccessStatusCode)
            {
                
                return RedirectToPage();
            }
            ModelState.AddModelError("", "B³¹d usuwania ksi¹¿ki.");
            await OnGetAsync();
            return Page();
        }
    }
}
