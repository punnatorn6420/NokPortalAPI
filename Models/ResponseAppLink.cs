using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class ResponseAppLink
    {
        [JsonPropertyName("appLink")]
        public string AppLink { get; set; } = string.Empty;
    }
}
