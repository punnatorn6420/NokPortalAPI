using System.Text.Json.Serialization;

namespace NokPortalAPI.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnvironmentType
    {
        Dev = 1,
        UAT = 2,
        Prod = 3
    }
}
