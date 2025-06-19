using System.Threading.Tasks;
using LibraryManagement.Core.Entities;
using LibraryManagement.Core.Interfaces;
using LibraryManagement.Infrastructure.Data;

namespace LibraryManagement.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly LibraryDbContext _context;

        public IRepository<Book> Books { get; }
        public IRepository<Author> Authors { get; }
        public IRepository<Member> Members { get; }
        public IRepository<Loan> Loans { get; }

        public UnitOfWork(LibraryDbContext context)
        {
            _context = context;
            Books = new BookRepository(context);
            Authors = new GenericRepository<Author>(context);
            Members = new GenericRepository<Member>(context);
            Loans = new GenericRepository<Loan>(context);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
