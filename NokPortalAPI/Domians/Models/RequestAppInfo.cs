using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class RequestAppInfo
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }
    }
}
