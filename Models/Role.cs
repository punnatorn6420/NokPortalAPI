using NokCore.Identity.Models;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents a role in the system.
    /// </summary>
    public class Role : IRole
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the collection of user roles.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Gets or sets the collection of role privileges.
        /// </summary>
        public ICollection<RolePermission> RolePrivileges { get; set; } = new List<RolePermission>();
    }
}
