using Newtonsoft.Json;
using NokCore.Identity.Models;

namespace NokPortalAPI.Models
{
    public class User : IUser
    {
        public int Id { get; set; } = 0;
        public string ObjectId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public bool Active { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the collection of user roles.
        /// </summary>
        [JsonIgnore]
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for the assigned applications.
        /// </summary>
        public IList<AssignedUserAppRole> AssignedApps { get; set; } = new List<AssignedUserAppRole>();
    }
}
