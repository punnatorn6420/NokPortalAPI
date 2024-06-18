using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class RequestAppInfo
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }
    }
}
