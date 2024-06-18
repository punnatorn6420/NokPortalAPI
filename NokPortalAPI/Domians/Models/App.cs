using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class App
    {
        [JsonPropertyName("appId")]
        public int AppID { get; set; }

        [JsonPropertyName("name")]
        [Required]
        [MaxLength(255)]
        required public string Name { get; set; }

        [JsonPropertyName("header")]
        [Required]
        [MaxLength(255)]
        required public string Header { get; set; }

        [JsonPropertyName("subheader")]
        [MaxLength(255)]
        required public string Subheader { get; set; }

        [JsonPropertyName("detail")]
        [MaxLength(255)]
        required public string Detail { get; set; }

        [JsonPropertyName("image")]
        [MaxLength(255)]
        required public string Image { get; set; }
    }
}
