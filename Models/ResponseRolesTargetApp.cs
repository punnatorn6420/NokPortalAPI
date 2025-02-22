using System.Text.Json.Serialization;
using NokCore.Identity.Models;

namespace NokPortalAPI.Models
{
    public class ResponseRolesTargetApp
    {
        [JsonPropertyName("status")]
        public bool Status { get; set; }

        [JsonPropertyName("data")]
        public IEnumerable<Role> Data { get; set; } = Enumerable.Empty<Role>();
    }
}
