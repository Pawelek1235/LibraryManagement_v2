using System.Collections.Generic;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LibraryManagement.Grpc.Protos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;



namespace LibraryManagement.UI.Pages
{
    public class BookSearchModel : PageModel
    {
        [BindProperty]
        public string Query { get; set; }

        public List<BookDto> Results { get; set; }

        public void OnGet() { }

        public async Task OnPostAsync()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5002");
            var client = new BookSearch.BookSearchClient(channel);
            var reply = await client.SearchAsync(new SearchRequest { Title = Query });
            Results = new List<BookDto>(reply.Books);
        }
    }
}
