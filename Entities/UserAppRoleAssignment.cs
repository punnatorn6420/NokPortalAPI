using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a user to an application with a role.
    /// </summary>
    public class UserAppRoleAssignment
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// App ID
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// Role ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation property for the associated User.
        /// </summary>
        public User? User { get; set; }

        /// <summary>
        /// Navigation property for the associated App.
        /// </summary>
        public App? App { get; set; }
    }
}