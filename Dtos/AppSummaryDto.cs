using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data transfer object for application summary information.
    /// </summary>
    public class AppSummaryDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the application.
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
        /// Gets or sets the environment type of the application.
        /// </summary>
        [Required]
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the client URL of the application.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ClientUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the backend URL of the application.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string BackendUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the image URL of the application.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT expiry hours for the application.
        /// </summary>
        [Required]
        public int JwtExpiryHours { get; set; } = 8;

        /// <summary>
        /// Gets or sets remarks for the application.
        /// </summary>
        [AllowNull]
        [MaxLength(255)]
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the application is active.
        /// </summary>
        public bool Active { get; set; } = false;
    }
}