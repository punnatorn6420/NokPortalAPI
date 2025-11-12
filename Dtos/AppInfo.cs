using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Application information.
    /// </summary>
    public class AppInfo
    {
        /// <summary>
        /// Gets or sets the client URL.
        /// </summary>
        public string ClientUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Dev;

        /// <summary>
        /// Gets or sets the JWT token.
        /// </summary>
        public string JwtToken { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the JWT expiration time.
        /// </summary>
        public DateTime JwtExpiryTime { get; set; } = DateTime.MinValue;
    }
}