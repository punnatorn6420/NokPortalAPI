using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    /// <summary>
    /// Application environment model.
    /// </summary>
    public class AppEnv
    {
        /// <summary>
        /// Gets or sets the application ID.
        /// </summary>
        [JsonPropertyName("appId")]
        public int AppId { get; set; } = 0;

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        [JsonPropertyName("environment")]
        public EnumEnvironmentType Environment { get; set; }

        /// <summary>
        /// Gets or sets the base URL.
        /// </summary>
        [JsonPropertyName("baseURL")]
        public string BaseURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the additional.
        /// </summary>
        [JsonPropertyName("additional")]
        public string Additional { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secret key for JWT.
        /// </summary>
        [JsonIgnore]
        [JsonPropertyName("secretKey")]
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT hour limit.
        /// </summary>
        [JsonPropertyName("jwtHourLimit")]
        public int JwtHourLimit { get; set; }
    }
}
