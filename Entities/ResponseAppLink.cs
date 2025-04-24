using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    public class ResponseAppLink
    {
        [JsonPropertyName("appLink")]
        public string AppLink { get; set; } = string.Empty;
    }
}
