using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using LibraryManagement.Grpc.Protos;
using LibraryManagement.UI.Models;      // BookDto w projekcie UI
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LibraryManagement.UI.Pages
{
    public class BookSearchModel : PageModel
    {
        [BindProperty]
        public string Query { get; set; }

        public List<BookDto> Results { get; set; } = new();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Query))
            {
                ModelState.AddModelError(nameof(Query), "Podaj tytu³ do wyszukania.");
                return Page();
            }

            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5002");
                var client = new BookSearch.BookSearchClient(channel);
                var reply = await client.SearchAsync(new SearchRequest { Title = Query });

                Results = reply.Books
                    .Select(p => new BookDto
                    {
                        Id = p.Id,
                        Title = p.Title,
                        //authorId = p.Au
                         
                    })
                    .ToList();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"B³¹d API: {ex.Message}");
            }

            return Page();
        }
    }
}
