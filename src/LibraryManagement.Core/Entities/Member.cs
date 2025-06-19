namespace LibraryManagement.Core.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTime MembershipDate { get; set; }

        // nowe pola:
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string Role { get; set; }    // np. "Admin" albo "Member"

        public ICollection<Loan> Loans { get; set; }
    }
}
