using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// This class represents the request for registering an app user.
    /// </summary>
    public class UserAppAssignmentRequest
    {
        /// <summary>
        /// ID of the application.
        /// </summary>
        public int AppId { get; set; }

        /// <summary>
        /// ID of the user to be assigned.
        /// </summary>
        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        /// <summary>
        /// List of role IDs to be assigned to the user.
        /// </summary>
        [Required(ErrorMessage = "Roles are required and must not be empty.")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();

        /// <summary>
        /// Environment type for the app user assignment.
        /// </summary>
        [Required(ErrorMessage = "Environment is required.")]
        public EnvironmentType Environment { get; set; }
    }
}