using System;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Book> Books { get; }
        IRepository<Author> Authors { get; }
        IRepository<Member> Members { get; }
        IRepository<Loan> Loans { get; }

        Task<int> SaveChangesAsync();
    }
}
