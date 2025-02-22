using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NokCore.Api.JWT.Models;

namespace NokPortalAPI.Models
{
    public class ResponseAppsEnv
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;

        [JsonPropertyName("baseURL")]
        public string BaseURL { get; set; } = string.Empty;

        [JsonPropertyName("additional")]
        public string Additional { get; set; } = string.Empty;

        [JsonPropertyName("jwtTargetApp")]
        required public JwtResponse JwtTargetApp { get; set; }
    }
}
