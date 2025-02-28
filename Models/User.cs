using Newtonsoft.Json;
using NokCore.Identity.Models;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Models
{
    public class User : IUser
    {
        public int Id { get; set; } = 0;

        [MaxLength(255)]
        public string ObjectId { get; set; } = string.Empty;

        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(255)]
        public string JobTitle { get; set; } = string.Empty;

        [MaxLength(255)]
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
        public IList<UserAppRoleAssignment> AssignedApps { get; set; } = new List<UserAppRoleAssignment>();
    }
}
