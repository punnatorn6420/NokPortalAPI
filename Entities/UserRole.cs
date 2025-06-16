using Newtonsoft.Json;
using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a user and a role.
    /// </summary>
    public class UserRole : IUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navigation properties
        public User? User { get; set; } = null!;
        public Role? Role { get; set; } = null!;
    }
}
