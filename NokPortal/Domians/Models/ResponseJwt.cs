using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class ResponseJwt
    {
        [JsonPropertyName("token")]
        required public string Token { get; set; }

        [JsonPropertyName("expiresTime")]
        required public DateTime ExpiresTime { get; set; }
    }
}
