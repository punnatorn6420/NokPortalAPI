using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class ApiAppResponse
    {
        [JsonPropertyName("status")]

        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

    }
}
