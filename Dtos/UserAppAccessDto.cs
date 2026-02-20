namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Lightweight app information used in user search responses.
    /// </summary>
    public class UserAppAccessDto
    {
        /// <summary>
        /// Application ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Application name.
        /// </summary>
        public string Name { get; set; } = string.Empty;
    }
}