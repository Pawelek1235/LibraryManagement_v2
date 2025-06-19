using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages.Admin.Loans
{
    public class LoansIndexModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public IEnumerable<Loan> Loans { get; set; }

        public LoansIndexModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task OnGetAsync()
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            // Pobierz wszystkie po¿yczki, w REST API kontroler Loans powinien zwracaæ Book i Member
            Loans = await client.GetFromJsonAsync<List<Loan>>("https://localhost:5001/api/loans");
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["jwt"]}");
            await client.DeleteAsync($"https://localhost:5001/api/loans/{id}");
            return RedirectToPage();
        }
    }
}
