using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    public class App
    {
        [JsonPropertyName("appId")]
        public int AppId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("header")]
        [MaxLength(255)]
        public string Header { get; set; } = string.Empty;

        [JsonPropertyName("subheader")]
        [MaxLength(255)]
        public string Subheader { get; set; } = string.Empty;

        [JsonPropertyName("detail")]
        [MaxLength(255)]
        public string Detail { get; set; } = string.Empty;

        [JsonPropertyName("image")]
        [MaxLength(255)]
        public string Image { get; set; } = string.Empty;

        [JsonPropertyName("baseUrl")]
        [MaxLength(255)]
        public string BaseUrl { get; set; } = string.Empty;
    }
}
