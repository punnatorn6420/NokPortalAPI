using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class UserADInnerError
    {
        public DateTime Date { get; set; }

        [JsonPropertyName("request-id")]
        public string RequestId { get; set; } = string.Empty;

        [JsonPropertyName("client-request-id")]
        public string ClientRequestId { get; set; } = string.Empty;
    }
}
