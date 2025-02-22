using System.Text.Json.Serialization;

namespace NokPortalAPI.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumEnvironmentType
    {
        Dev = 1,
        UAT = 2,
        Prod = 3
    }
}
