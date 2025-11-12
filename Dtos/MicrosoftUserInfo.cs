using System.Text.Json.Serialization;

namespace NokPortalAPI.Dtos
{
    /// <summary>
    /// Represents the Microsoft user information.
    /// </summary>
    public class MicrosoftUserInfo
    {
        /// <summary>
        /// Gets or sets the OData context.
        /// </summary>
        [JsonPropertyName("@odata.context")]
        public required string ODataContext { get; set; }

        /// <summary>
        /// Gets or sets the business phone numbers.
        /// </summary>
        [JsonPropertyName("businessPhones")]
        public required List<string> BusinessPhones { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [JsonPropertyName("displayName")]
        public required string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        [JsonPropertyName("givenName")]
        public required string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the job title.
        /// </summary>
        [JsonPropertyName("jobTitle")]
        public required string JobTitle { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [JsonPropertyName("mail")]
        public required string Mail { get; set; }

        /// <summary>
        /// Gets or sets the mobile phone number.
        /// </summary>
        [JsonPropertyName("mobilePhone")]
        public required string MobilePhone { get; set; }

        /// <summary>
        /// Gets or sets the office location.
        /// </summary>
        [JsonPropertyName("officeLocation")]
        public required string OfficeLocation { get; set; }

        /// <summary>
        /// Gets or sets the preferred language.
        /// </summary>
        [JsonPropertyName("preferredLanguage")]
        public required string PreferredLanguage { get; set; }

        /// <summary>
        /// Gets or sets the surname.
        /// </summary>
        [JsonPropertyName("surname")]
        public required string Surname { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        [JsonPropertyName("userPrincipalName")]
        public required string UserPrincipalName { get; set; }

        /// <summary>
        /// ID of the user.
        /// </summary>
        [JsonPropertyName("id")]
        public required string Id { get; set; }
    }
}