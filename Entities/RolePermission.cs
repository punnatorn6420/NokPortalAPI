using Newtonsoft.Json;
using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Entities
{
    public class RolePermission : IRolePermission
    {
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the permission ID.
        /// </summary>
        public int PermissionId { get; set; }

        // Navigation properties
        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
