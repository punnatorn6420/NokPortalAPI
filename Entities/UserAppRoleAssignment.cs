using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a user to an application with a role.
    /// </summary>
    public class UserAppRoleAssignment
    {
        public int UserId { get; set; }
        public int AppId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public App? App { get; set; }
    }
}
