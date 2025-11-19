using NokAir.Core.Abstractions.Entities.Rbac;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a role and a permission.
    /// </summary>
    public class RolePermission : IRolePrivilege
    {
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the permission ID.
        /// </summary>
        public int PermissionId { get; set; }

        /// <summary>
        /// Navigation property for the associated Role.
        /// </summary>
        public Role Role { get; set; } = null!;

        /// <summary>
        /// Navigation property for the associated Permission.
        /// </summary>
        public Permission Permission { get; set; } = null!;
    }
}