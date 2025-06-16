using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Entities
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

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserAppRoleAssignment> UserAppRoleAssignments { get; set; } = new List<UserAppRoleAssignment>();
    }
}
