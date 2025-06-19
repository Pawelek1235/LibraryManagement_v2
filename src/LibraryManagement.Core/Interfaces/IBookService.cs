using System.Collections.Generic;
using System.Threading.Tasks;
using LibraryManagement.Core.Entities;

namespace LibraryManagement.Core.Interfaces
{
    public interface IBookService
    {
        /// <summary>
        /// Wyszukuje książki po fragmencie tytułu.
        /// </summary>
        Task<IEnumerable<Book>> SearchByTitleAsync(string title);
    }
}
