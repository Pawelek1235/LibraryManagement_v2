using System;
using System.Collections.Generic;

namespace LibraryManagement.Core.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public DateTime PublishedDate { get; set; }

        public int AuthorId { get; set; }
        public Author Author { get; set; }

        public int CategoryId { get; set; }

        public ICollection<Loan> Loans { get; set; }
    }
}
