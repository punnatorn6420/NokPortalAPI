using System.ComponentModel.DataAnnotations;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Represents a request for a Microsoft token.
    /// </summary>
    public class MicrosoftTokenDto
    {
        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; } = string.Empty;
    }
}