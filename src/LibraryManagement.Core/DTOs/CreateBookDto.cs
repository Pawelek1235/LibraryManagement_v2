// LibraryManagement.Core/DTOs/CreateBookDto.cs
using System.ComponentModel.DataAnnotations;

namespace LibraryManagement.Core.DTOs
{
    public class CreateBookDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public int AuthorId { get; set; }
    }
}
