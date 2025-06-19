using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class BookRepository : GenericRepository<Book>, IRepository<Book>
    {
        public BookRepository(LibraryDbContext context)
            : base(context) { }

        /// <summary>
        /// Dodatkowa metoda specyficzna dla repozytorium Book:
        /// wyszukiwanie po fragmencie tytułu wraz z załadowaniem autora.
        /// </summary>
        public async Task<IEnumerable<Book>> SearchByTitleAsync(string title)
        {
            return await _context.Books
                                 .Where(b => b.Title.Contains(title))
                                 .Include(b => b.Author)
                                 .ToListAsync();
        }
    }
}
