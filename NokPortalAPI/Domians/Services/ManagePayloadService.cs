using System.Reflection;
using System.Text.Json.Serialization;
using NokCore.Identity.Models;

namespace NokPortal.Domians.Services
{
    public class ManagePayloadService : IManagePayloadService
    {
        public string GetJsonPropertyName<T>(string propertyName)
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName);
            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{typeof(T)}'.");
            }

            var jsonPropertyNameAttribute = property.GetCustomAttribute<JsonPropertyNameAttribute>();
            if (jsonPropertyNameAttribute != null)
            {
                return jsonPropertyNameAttribute.Name;
            }
            return property.Name;
        }

        public int GetUserIdFromJwtDecode(HttpContext payload)
        {
            var userIdString = payload.Items[GetJsonPropertyName<JwtData>(nameof(JwtData.UserId))] as string
                   ?? throw new ArgumentNullException("Not found userId");

            int userId;
            if (!int.TryParse(userIdString, out userId))
            {
                throw new ArgumentException("Invalid userId");
            }

            return userId;
        }
    }
}
