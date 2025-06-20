namespace LibraryManagement.UI.Models
{
    public class BookDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        // Dodaj lub upewnij się, że są te właściwości:
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;           // 'Isbn' z małą s
        public DateTime PublishDate { get; set; }
    }
}
