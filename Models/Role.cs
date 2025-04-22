using NokAir.Core.Interfaces.Rbac.Entities;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents a role in the system.
    /// </summary>
    public class Role : IRole
    {
        public int Id { get; set; } = 0;

        [MaxLength(255)]
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
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
