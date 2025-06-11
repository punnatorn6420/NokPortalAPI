using NokPortalAPI.Enums;

namespace NokPortalAPI.Entities
{
    public class AppInfo
    {
        public string ClientUrl { get; set; } = string.Empty;

        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Dev;

        public string JwtToken { get; set; } = string.Empty;

        public DateTime JwtExpiryTime { get; set; } = DateTime.MinValue;
    }
}
