using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class RequestAddUserApp
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("roles")]
        public IEnumerable<int> Roles { get; set; } = Enumerable.Empty<int>();

        [JsonPropertyName("environment")]
        public EnumEnvironmentType Environment { get; set; }
    }
}
