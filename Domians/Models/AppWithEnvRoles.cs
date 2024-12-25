using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class AppWithEnvRoles : App
    {
        [JsonPropertyName("environmentRoles")]
        public IEnumerable<EnvRoles> EnvironmentRoles { get; set; } = Enumerable.Empty<EnvRoles>();
    }
}
