using LibraryManagement.Contracts;    
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages
{
    public class BookSearchModel : PageModel
    {
        private readonly HttpClient _client;

        public BookSearchModel(IHttpClientFactory httpFactory)
        {
            _client = httpFactory.CreateClient("ApiClient");
        }

        [BindProperty]
        public string Query { get; set; }

        public List<BookDto> Results { get; set; } = new();

        public void OnGet()
        {
           
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
             
                var url = $"api/books/search?title={Uri.EscapeDataString(Query)}";

               
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
