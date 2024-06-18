using System.Reflection;
using System.Text.Json.Serialization;

namespace NokPortal.Domians.Services
{
    public class JsonHelperService : IJsonHelperService
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
    }
}
