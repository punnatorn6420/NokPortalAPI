using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    public class MicrosoftUserInfo
    {
        [JsonPropertyName("@odata.context")]
        required public string ODataContext { get; set; }

        [JsonPropertyName("businessPhones")]
        required public List<string> BusinessPhones { get; set; }

        [JsonPropertyName("displayName")]
        required public string DisplayName { get; set; }

        [JsonPropertyName("givenName")]
        required public string GivenName { get; set; }

        [JsonPropertyName("jobTitle")]
        required public string JobTitle { get; set; }

        [JsonPropertyName("mail")]
        required public string Mail { get; set; }

        [JsonPropertyName("mobilePhone")]
        required public string MobilePhone { get; set; }

        [JsonPropertyName("officeLocation")]
        required public string OfficeLocation { get; set; }

        [JsonPropertyName("preferredLanguage")]
        required public string PreferredLanguage { get; set; }

        [JsonPropertyName("surname")]
        required public string Surname { get; set; }

        [JsonPropertyName("userPrincipalName")]
        required public string UserPrincipalName { get; set; }

        [JsonPropertyName("id")]
        required public string Id { get; set; }
    }
}
