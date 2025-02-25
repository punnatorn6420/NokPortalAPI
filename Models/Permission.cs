using NokCore.Identity.Models;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents a privilege in the system.
    /// </summary>
    public class Permission : IPermission
    {
        public int Id { get; set; } = 0;

        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of role privileges.
        /// </summary>
        public ICollection<RolePermission> RolePrivileges { get; set; } = new List<RolePermission>();
    }
}
