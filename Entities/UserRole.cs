using NokAir.Core.Abstractions.Entities.Rbac;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the association between a user and a role.
    /// </summary>
    public class UserRole : IUserRole
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Role ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Navigation properties
        /// </summary>
        public User? User { get; set; } = new User();

        /// <summary>
        /// Navigation property for the associated Role.
        /// </summary>
        public Role? Role { get; set; } = new Role();
    }
}