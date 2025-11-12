using System.Text.Json.Serialization;

namespace NokPortalAPI.Enums
{
    /// <summary>
    /// Types of environments.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnvironmentType
    {
        /// <summary>
        /// Development environment
        /// </summary>
        Dev = 1,

        /// <summary>
        /// User Acceptance Testing environment
        /// </summary>
        UAT = 2,

        /// <summary>
        /// Production environment
        /// </summary>
        Prod = 3
    }
}