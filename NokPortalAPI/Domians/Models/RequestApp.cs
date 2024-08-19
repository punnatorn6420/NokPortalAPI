using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Domains.Models
{
    [Obsolete("This class is obsolete, use AppEnv instead.")]
    public class RequestApp
    {
        [Required]
        [MaxLength(255)]
        required public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        required public string Header { get; set; }

        [MaxLength(255)]
        required public string Subheader { get; set; }

        [MaxLength(255)]
        required public string Detail { get; set; }

        [MaxLength(255)]
        required public string Image { get; set; }

        [MaxLength(255)]
        required public string BaseUrl { get; set; }
    }
}
