using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class AppWithRoles
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("header")]
        public string Header { get; set; } = string.Empty;

        [JsonPropertyName("subheader")]
        public string Subheader { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("baseUrl")]
        public string BaseUrl { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public List<int> Roles { get; set; } = new List<int>();
    }
}
