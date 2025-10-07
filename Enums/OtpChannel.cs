using System.Text.Json.Serialization;

namespace NokPortalAPI.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OtpChannel
    {
        Email,
        Sms
    }
}
