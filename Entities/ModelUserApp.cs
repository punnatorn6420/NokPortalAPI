using NokPortalAPI.Enums;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    public class ModelUserApp
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roleId")]
        public AppRole RoleId { get; set; }
    }
}