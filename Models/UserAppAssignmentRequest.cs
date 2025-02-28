using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// This class represents the request for registering an app user.
    /// </summary>
    public class UserAppAssignmentRequest
    {
        public int AppId { get; set; }

        [Required(ErrorMessage = "UserId is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Roles are required and must not be empty.")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();


        [Required(ErrorMessage = "Environment is required.")]
        public EnvironmentType Environment { get; set; }
    }
}
