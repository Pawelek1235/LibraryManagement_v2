using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Authors
{
    public class IndexModel : PageModel
    {
        private readonly IHttpClientFactory _httpFactory;
        public IEnumerable<Author> Authors { get; set; }

        public IndexModel(IHttpClientFactory httpFactory)
            => _httpFactory = httpFactory;

        public async Task OnGetAsync()
        {
            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            Authors = await client.GetFromJsonAsync<List<Author>>("https://localhost:5001/api/authors");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _httpFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            await client.DeleteAsync($"https://localhost:5001/api/authors/{id}");
            return RedirectToPage();
        }
    }
}
