using System;

namespace LibraryManagement.API.Models
{
    public class AuthResponse
    {
        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public string Role { get; set; }
        public int UserId { get; set; }   
    }

}
