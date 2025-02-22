using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class RequestRolesTargetApp
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("roles")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();

        [JsonPropertyName("department")]
        public string Department { get; set; } = string.Empty;

        [JsonPropertyName("position")]
        public string Position { get; set; } = string.Empty;
    }
}
