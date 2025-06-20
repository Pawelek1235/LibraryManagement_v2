namespace LibraryManagement.Contracts
{
    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }
}
