namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a user and an application role.
    /// </summary>
    public class UserAppRole
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// Gets or sets the application role ID.
        /// </summary>
        public int AppRoleId { get; set; }
    }
}
