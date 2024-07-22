using System.Text.Json.Serialization;
using NokCore.Identity.Models;

namespace NokPortalAPI.Domains.Models
{
    public class UserApps
    {
        [JsonPropertyName("app")]
        required public IEnumerable<AppWithRoles> App { get; set; }

        [JsonPropertyName("user")]
        required public User User { get; set; }
    }
}
