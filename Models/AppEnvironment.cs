using NokPortalAPI.Enums;
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
        /// Surrogate key for the application environment.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        [Required]
        public int AppId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the application. This is a navigation property.
        /// </summary>
        [JsonIgnore]
        public App App { get; set; } = new App();

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        /// 
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        [MaxLength(255)]
        public string BaseURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secret key for JWT.
        /// </summary>
        [JsonIgnore]
        [MaxLength(255)]
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
    }
}
