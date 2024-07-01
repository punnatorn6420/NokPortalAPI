using System.Text.Json.Serialization;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;

namespace NokPortalAPI.Domians.Models
{
    public class UserApps
    {
        [JsonPropertyName("app")]
        required public IEnumerable<ModelApp> App { get; set; }

        [JsonPropertyName("user")]
        required public User User { get; set; }
    }
}
