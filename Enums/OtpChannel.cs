using System.Text.Json.Serialization;

namespace NokPortalAPI.Enums
{
    /// <summary>
    /// OTP delivery channels.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum OtpChannel
    {
        /// <summary>
        /// Email channel
        /// </summary>
        Email,

        /// <summary>
        /// SMS channel
        /// </summary>
        Sms,
    }
}