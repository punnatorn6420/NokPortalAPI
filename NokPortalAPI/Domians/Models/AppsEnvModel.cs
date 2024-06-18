using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class AppsEnvModel
    {
        [JsonPropertyName("appID")]
        public int AppID { get; set; }

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;

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
