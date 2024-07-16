using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Domains.Models
{
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
    }
}
