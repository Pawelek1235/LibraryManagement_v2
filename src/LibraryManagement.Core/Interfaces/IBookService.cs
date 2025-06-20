using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces
{
    public interface IBookService
    {   
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);
    }
}
