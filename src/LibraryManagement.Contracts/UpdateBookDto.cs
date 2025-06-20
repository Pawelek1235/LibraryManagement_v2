namespace LibraryManagement.Contracts
{
    public class UpdateBookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public int AuthorId { get; set; }
    }
}
