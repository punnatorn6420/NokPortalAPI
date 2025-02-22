using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class ResponseRetireveToken
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("environment")]
        public string Environment { get; set; } = string.Empty;
    }
}
