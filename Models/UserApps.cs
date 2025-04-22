using System.Text.Json.Serialization;
using NokAir.Core.Interfaces.Rbac.Entities;

namespace NokPortalAPI.Models
{
    /// <summary>
    /// Represents the user and the apps that the user has access to.
    /// </summary>
    public class UserApps
    {
        /// <summary>
        /// A list of apps with roles that the user has access to.
        /// </summary>
        [JsonPropertyName("app")]
        required public IEnumerable<AppWithRoles> App { get; set; }

        /// <summary>
        /// The user that has access to the apps.
        /// </summary>
        [JsonPropertyName("user")]
        required public IUser User { get; set; }
    }
}
