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

        /// <summary>
        /// Gets or sets the role. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public Role Role { get; set; } = new Role();

        /// <summary>
        /// Gets or sets the privilege. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public Permission Permission { get; set; } = new Permission();
    }
}
