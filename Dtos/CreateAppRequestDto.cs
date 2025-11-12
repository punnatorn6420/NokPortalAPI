using NokPortalAPI.Enums;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Data transfer object for creating a new application.
    /// </summary>
    public class CreateAppRequestDto
    {
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the header of the application.
        /// </summary>
        public string Header { get; set; } = null!;

        /// <summary>
        /// Gets or sets the subheader of the application.
        /// </summary>
        public string Subheader { get; set; } = null!;

        /// <summary>
        /// Gets or sets the environment type of the application.
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; }

        /// <summary>
        /// Gets or sets the client URL of the application.
        /// </summary>
        public string ClientUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the backend URL of the application.
        /// </summary>
        public string BackendUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the image URL of the application.
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the secret key of the application.
        /// </summary>
        public string SecretKey { get; set; } = null!;

        /// <summary>
        /// Gets or sets the JWT expiry hours.
        /// </summary>
        public int JwtExpiryHours { get; set; }

        /// <summary>
        /// Gets or sets the remark of the application.
        /// </summary>
        public string Remark { get; set; } = null!;
    }
}