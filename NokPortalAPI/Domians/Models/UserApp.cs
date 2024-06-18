using System.Text.Json.Serialization;

namespace NokPortal.Domains.Models
{
    public class UserApp
    {

        [JsonPropertyName("appId")]
        public int AppID { get; set; }

        [JsonPropertyName("userId")]
        public int UserID { get; set; }
    }
}