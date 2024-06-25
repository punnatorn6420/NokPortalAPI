using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Domians.Models
{
    public class RequestAddUserApp
    {
        [JsonPropertyName("userId")]
        [Required]
        required public int UserId { get; set; }
    }
}
