using NokAir.Core.Abstractions.Entities.Rbac;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : IUser
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Object ID from external identity provider.
        /// </summary>
        public string ObjectId { get; set; } = string.Empty;

        /// <summary>
        /// First name of the user.
        /// </summary>
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Job title of the user.
        /// </summary>
        public string JobTitle { get; set; } = string.Empty;

        /// <summary>
        /// Department of the user.
        /// </summary>
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the user is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Date and time when the user was last modified.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property for user roles.
        /// </summary>
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        /// <summary>
        /// Navigation property for user-app-role assignments.
        /// </summary>
        public IList<UserAppRoleAssignment> UserAppRoleAssignments { get; set; } = new List<UserAppRoleAssignment>();
    }
}