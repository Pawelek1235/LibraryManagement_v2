namespace LibraryManagement.Infrastructure.Configuration
{
    public class AppSettings
    {
        /// <summary>
        /// Sekret używany do podpisywania tokenów JWT (powinien być długi i losowy).
        /// </summary>
        public string JwtSecret { get; set; }

        /// <summary>
        /// Czas życia tokenu w minutach.
        /// </summary>
        public int JwtExpiresMinutes { get; set; }
    }
}
