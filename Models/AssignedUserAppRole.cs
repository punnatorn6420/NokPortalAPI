using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents the association between a user to an application with a role.
    /// </summary>
    public class AssignedUserAppRole
    {
        /// <summary>
        /// Surrogate key for the assigned user application.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public User User { get; set; } = new User();

        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// Gets or sets application. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public App App { get; set; } = new App();

        /// <summary>
        /// Gets or sets the application role ID.
        /// </summary>
        public int AppRoleId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the user was assigned to the application role.
        /// </summary>
        public DateTime AssignedDate { get; set; } = DateTime.Now;
    }
}
