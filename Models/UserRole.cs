using Newtonsoft.Json;
using NokCore.Identity.Models;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents the association between a user and a role.
    /// </summary>
    public class UserRole : IUserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        /// <summary>
        /// Gets or sets the user. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public User User { get; set; } = new User();

        /// <summary>
        /// Gets or sets role. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public Role Role { get; set; } = new Role();
    }
}
