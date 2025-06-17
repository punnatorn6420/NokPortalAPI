using System.Text.Json.Serialization;
using NokPortalAPI.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace NokPortalAPI.Entities
{
    /// <summary>
    /// Represents the application.
    /// </summary>
    public class App
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Header { get; set; } = string.Empty;

        public string Subheader { get; set; } = string.Empty;

        public EnvironmentType EnvironmentType { get; set; } = EnvironmentType.Dev;

        public string ClientUrl { get; set; } = string.Empty;

        public string BackendUrl { get; set; } = string.Empty;

        public string SecretKey { get; set; } = string.Empty;

        public int JwtExpiryHours { get; set; } = 8;

        public string Remark { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public bool Active { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ModifiedAt { get; set; } = DateTime.Now;


        // Navigation properties
        public ICollection<UserAppRoleAssignment> UserAppRoleAssignments { get; set; } = new List<UserAppRoleAssignment>();
    }
}
