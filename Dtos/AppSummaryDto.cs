using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NokPortalAPI.Dtos
{
    public class AppSummaryDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string Header { get; set; } = string.Empty;

        [AllowNull]
        [MaxLength(255)]
        public string Subheader { get; set; } = string.Empty;

        [Required]
        public EnvironmentType EnvironmentType { get; set; }

        [Required]
        [MaxLength(255)]
        public string ClientUrl { get; set; } = string.Empty;
        [Required]
        [MaxLength(255)]
        public string BackendUrl { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; } = "";

        [Required]
        public int JwtExpiryHours { get; set; } = 8;

        [AllowNull]
        [MaxLength(255)]
        public string Remark { get; set; } = string.Empty;

        public bool Active { get; set; } = false;
    }
}
