using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Members
{
    public class MembersEditModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        [BindProperty]
        public Member Member { get; set; }

        public MembersEditModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync(int? id)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            if (id.HasValue)
                Member = await client.GetFromJsonAsync<Member>($"https://localhost:5001/api/members/{id}");
            else
                Member = new Member();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Jeœli model jest nieprawid³owy, wróæ na tê sam¹ stronê
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            if (Member.Id == 0)
            {
                // nowy cz³onek
                await client.PostAsJsonAsync("https://localhost:5001/api/members", Member);
            }
            else
            {
                // edycja istniej¹cego
                await client.PutAsJsonAsync($"https://localhost:5001/api/members/{Member.Id}", Member);
            }

            // Po zapisie przekieruj z powrotem na listê
            return RedirectToPage("MembersIndex");
        }
    }
}
