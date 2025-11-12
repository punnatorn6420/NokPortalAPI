using NokPortalAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data Transfer Object for User.
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; } = 0;

        /// <summary>
        /// Object ID of the user.
        /// </summary>
        [MaxLength(255)]
        public string ObjectId { get; set; } = string.Empty;

        /// <summary>
        /// First name of the user.
        /// </summary>
        [MaxLength(255)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Last name of the user.
        /// </summary>
        [MaxLength(255)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Email of the user.
        /// </summary>
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Job title of the user.
        /// </summary>
        [MaxLength(255)]
        public string JobTitle { get; set; } = string.Empty;

        /// <summary>
        /// Department of the user.
        /// </summary>
        [MaxLength(255)]
        public string Department { get; set; } = string.Empty;

        /// <summary>
        /// Role of the user.
        /// </summary>
        [MaxLength(255)]
        public string Role { get; set; } = string.Empty;

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
    }
}