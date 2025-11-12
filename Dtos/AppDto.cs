using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data transfer object for application information.
    /// </summary>
    public class AppDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the header of the application.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the subheader of the application.
        /// </summary>
        [AllowNull]
        [MaxLength(255)]
        public string Subheader { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        [Required]
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ClientUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the backend URL.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string BackendUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secret key for JWT.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT expiration hour. Default is 8 hours.
        /// </summary>
        [Required]
        public int JwtExpiryHours { get; set; } = 8;

        /// <summary>
        /// Gets or sets remarks about the application.
        /// </summary>
        [AllowNull]
        [MaxLength(255)]
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the user is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Gets or sets the date and time when the user was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date and time when the user was last modified.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}