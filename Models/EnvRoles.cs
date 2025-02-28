using NokPortalAPI.Enums;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class EnvRoles
    {
        [JsonPropertyName("environment")]
        public EnvironmentType Environment { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();

    }
}
