using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents a role in the system.
    /// </summary>
    public class Role : IRole
    {
        /// <summary>
        /// Role ID
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// Name of the role.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the role is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Date and time when the role was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Date and time when the role was last modified.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property for the associated UserRoles.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for the associated RolePermissions.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}