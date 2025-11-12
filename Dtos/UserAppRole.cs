namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data transfer object for user application roles.
    /// </summary>
    public class UserAppRoleDto
    {
        /// <summary>
        /// ID of the user.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        ///  ID of the application.
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// List of assigned role IDs for the user in the application.
        /// </summary>
        public IList<int> AssignedRoleIds { get; set; } = new List<int>();
    }
}