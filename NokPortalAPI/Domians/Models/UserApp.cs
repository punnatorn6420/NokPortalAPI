using System.Text.Json.Serialization;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domains.Models
{
    public class UserApp
    {

        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roleId")]
        public UserRole RoleId { get; set; }
    }
}