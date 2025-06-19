using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Members
{
    public class MembersIndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IEnumerable<Member> Members { get; set; }

        public MembersIndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            Members = await client.GetFromJsonAsync<List<Member>>("https://localhost:5001/api/members");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            await client.DeleteAsync($"https://localhost:5001/api/members/{id}");
            return RedirectToPage();
        }
    }
}
