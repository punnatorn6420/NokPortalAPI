using Newtonsoft.Json;
using NokAir.Core.Interfaces.Rbac.Entities;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Entities
{
    public class User : IUser
    {
        public int Id { get; set; }

        public string ObjectId { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string JobTitle { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public bool Active { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public IList<UserAppRoleAssignment> UserAppRoleAssignments { get; set; } = new List<UserAppRoleAssignment>();
    }
}
