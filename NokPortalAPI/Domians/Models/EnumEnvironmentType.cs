using System.Text.Json.Serialization;

namespace NokPortalAPI.Domains.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumEnvironmentType
    {
        Dev,
        UAT,
        Prod
    }
}
