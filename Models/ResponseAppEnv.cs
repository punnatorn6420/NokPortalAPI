using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class ResponseAppEnv
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
