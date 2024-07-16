using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class ModelUserApp
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roleId")]
        public EnumUserRole RoleId { get; set; }
    }
}