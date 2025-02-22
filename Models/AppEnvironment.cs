using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents the application environment.
    /// </summary>
    public class AppEnvironment
    {
        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        [Required]
        public int AppId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        public EnumEnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        public string BaseURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secret key for JWT.
        /// </summary>
        [JsonIgnore]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT expiration hour. Default is 8 hours.
        /// </summary>
        public int JwtExpirationHour { get; set; } = 8;

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
        /// Gets or sets the application. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public App AssociatedApp { get; set; } = new App();
    }
}
