using System.Text.Json.Serialization;

namespace NokPortalAPI.Domians.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumEnvironmentType
    {
        Dev,
        UAT,
        Prod
    }
}
