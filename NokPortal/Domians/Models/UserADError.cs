using System.Text.Json.Serialization;

namespace NokPortal.Domians.Models
{
    public class UserADError
    {
        public string Code { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        [JsonPropertyName("innerError")]
        required public UserADInnerError InnerError { get; set; }
    }
}
