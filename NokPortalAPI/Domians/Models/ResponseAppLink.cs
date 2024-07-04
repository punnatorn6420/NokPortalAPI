using System.Text.Json.Serialization;

namespace NokPortalAPI.Domians.Models
{
    public class ResponseAppLink
    {
        [JsonPropertyName("appLink")]
        public string AppLink { get; set; } = string.Empty;
    }
}
