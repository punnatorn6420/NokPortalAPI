using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NokCore.Api.JwtToken.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Models
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
        required public ResponseJwt JwtTargetApp { get; set; }
    }
}
