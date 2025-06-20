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
           
            if (!ModelState.IsValid)
                return Page();

            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");

            if (Member.Id == 0)
            {
                
                await client.PostAsJsonAsync("https://localhost:5001/api/members", Member);
            }
            else
            {
                
                await client.PutAsJsonAsync($"https://localhost:5001/api/members/{Member.Id}", Member);
            }

       
            return RedirectToPage("MembersIndex");
        }
    }
}
