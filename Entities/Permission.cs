using NokAir.Core.Interfaces.Rbac.Entities;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents a privilege in Nok Portal system.
    /// </summary>
    public class Permission : IPermission
    {
        /// <summary>
        /// Permission ID
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// Name of the permission.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the permission is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Date and time when the permission was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Date and time when the permission was last modified.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property for the associated RolePermissions.
        /// </summary>
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}