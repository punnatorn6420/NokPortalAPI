using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class AppWithEnvRoles : App
    {
        [JsonPropertyName("environmentRoles")]
        public IEnumerable<EnvRoles> EnvironmentRoles { get; set; } = Enumerable.Empty<EnvRoles>();
    }
}
