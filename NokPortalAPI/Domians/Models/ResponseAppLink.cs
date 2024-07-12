using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class ResponseAppLink
    {
        [JsonPropertyName("appLink")]
        public string AppLink { get; set; } = string.Empty;
    }
}
