using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    public class UserAppWithEnvRoles
    {
        [JsonPropertyName("app")]

        public IEnumerable<AppWithEnvRoles> AppWithEnvRoles { get; set; } = Enumerable.Empty<AppWithEnvRoles>();

        [JsonPropertyName("user")]

        public User User { get; set; } = new User();
    }
}
