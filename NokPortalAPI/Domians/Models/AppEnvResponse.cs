using System.Text.Json.Serialization;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Models
{
    public class AppEnvResponse
    {
        [JsonPropertyName("appId")]
        public int? AppId { get; set; }

        [JsonPropertyName("environment")]
        public EnumEnvironmentType Environment { get; set; }

        [JsonPropertyName("baseURL")]
        public string BaseURL { get; set; } = string.Empty;

        [JsonPropertyName("additional")]
        public string Additional { get; set; } = string.Empty;

        [JsonPropertyName("jwtHourLimit")]
        public int JwtHourLimit { get; set; }
    }
}
