using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents the application.
    /// </summary>
    public class App
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Header { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Subheader { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Detail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        [MaxLength(255)]
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        [MaxLength(255)]
        public string BaseUrl { get; set; } = string.Empty;

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

        /// <summary>
        /// Gets or sets the application environments.
        /// </summary>
        public ICollection<AppEnvironment> Environments { get; set; } = new List<AppEnvironment>();

        /// <summary>
        /// Gets or sets the assigned user applications.
        /// </summary>
        public ICollection<AssignedUserAppRole> AssignedApps { get; set; } = new List<AssignedUserAppRole>();
    }
}
