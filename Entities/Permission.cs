using NokAir.Core.Interfaces.Rbac.Entities;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents a privilege in Nok Portal system.
    /// </summary>
    public class Permission : IPermission
    {
        public int Id { get; set; } = 0;

        public string Name { get; set; } = string.Empty;

        public bool Active { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
