using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class EnvRoles
    {
        [JsonPropertyName("environment")]
        public EnumEnvironmentType Environment { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();

    }
}
