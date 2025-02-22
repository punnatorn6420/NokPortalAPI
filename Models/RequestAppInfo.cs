using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class RequestAppInfo
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }
    }
}
