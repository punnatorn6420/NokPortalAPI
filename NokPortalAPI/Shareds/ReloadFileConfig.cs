using Newtonsoft.Json;

namespace NokPortalAPI.Shareds
{
    public class ReloadFileConfig
    {
        public IConfiguration ReloadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        public string GetConfigurationJson()
        {
            var config = ReloadConfiguration();
            var configDict = config.AsEnumerable();

            // Create a dictionary to store the configuration key-value pairs
            var dictionary = new Dictionary<string, string>();
            foreach (var kvp in configDict)
            {
                dictionary[kvp.Key] = kvp.Value ?? string.Empty;
            }

            // Convert the dictionary to a JSON string
            return JsonConvert.SerializeObject(dictionary, Formatting.Indented);
        }
    }
}
