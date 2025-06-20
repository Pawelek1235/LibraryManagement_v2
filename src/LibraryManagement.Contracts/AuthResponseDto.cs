namespace LibraryManagement.Contracts
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; } = string.Empty;
    }
}