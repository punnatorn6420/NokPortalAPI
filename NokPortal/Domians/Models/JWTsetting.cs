using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class JWTsetting
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("applicationId")]
        public int? ApplicationId { get; set; }
    }
}
