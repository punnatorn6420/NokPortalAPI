using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the application.
    /// </summary>
    public class App
    {
        /// <summary>
        /// App ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the app.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Header of the app.
        /// </summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>
        /// Subheader of the app.
        /// </summary>
        public string Subheader { get; set; } = string.Empty;

        /// <summary>
        /// Environment type of the app.
        /// </summary>
        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Dev;

        /// <summary>
        /// Client URL of the app.
        /// </summary>
        public string ClientUrl { get; set; } = string.Empty;

        /// <summary>
        /// Backend URL of the app.
        /// </summary>
        public string BackendUrl { get; set; } = string.Empty;

        /// <summary>
        /// Secret key for the app.
        /// </summary>
        public string SecretKey { get; set; } = string.Empty;

        /// <summary>
        /// JWT expiry time in hours.
        /// </summary>
        public int JwtExpiryHours { get; set; } = 8;

        /// <summary>
        /// Remarks about the app.
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the permission is active.
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Indicates whether the app is active.
        /// </summary>
        public bool Active { get; set; } = false;

        /// <summary>
        /// Date and time when the app was created.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Date and time when the app was last modified.
        /// </summary>
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Navigation property for the associated UserAppRoleAssignments.
        /// </summary>
        public ICollection<UserAppRoleAssignment> UserAppRoleAssignments { get; set; } = new List<UserAppRoleAssignment>();
    }
}