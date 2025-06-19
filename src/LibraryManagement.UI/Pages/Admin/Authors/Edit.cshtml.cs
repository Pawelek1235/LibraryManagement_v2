using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Authors
{
    public class EditModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;

        [BindProperty]
        public Author Author { get; set; }

        public EditModel(IHttpClientFactory httpFactory)
            => _httpFactory = httpFactory;

        public async Task OnGetAsync(int? id)
        {
            if (id.HasValue)
            {
                var client = _httpFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
                Author = await client.GetFromJsonAsync<Author>($"https://localhost:5001/api/authors/{id}");
            }
            else
            {
                Author = new Author();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            if (Author.Id == 0)
                await client.PostAsJsonAsync("https://localhost:5001/api/authors", Author);
            else
                await client.PutAsJsonAsync($"https://localhost:5001/api/authors/{Author.Id}", Author);

            return RedirectToPage("Index");
        }
    }
}
