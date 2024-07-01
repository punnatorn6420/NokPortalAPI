using System.Text.Json.Serialization;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Models
{
    public class AppEnv
    {
        [JsonPropertyName("appId")]
        public int? AppId { get; set; }

        [JsonPropertyName("environment")]
        public EnumEnvironmentType Environment { get; set; }

        [JsonPropertyName("baseURL")]
        public string BaseURL { get; set; } = string.Empty;

        [JsonPropertyName("additional")]
        public string Additional { get; set; } = string.Empty;

        [JsonPropertyName("secretKey")]
        public string SecretKey { get; set; } = string.Empty;

        [JsonPropertyName("jwtHourLimit")]
        public int JwtHourLimit { get; set; }
    }
}
