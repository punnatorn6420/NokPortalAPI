using Newtonsoft.Json;

namespace NokPortalAPI.Shareds
{
    /// <summary>
    /// Class for reloading file configuration.
    /// </summary>
    public class ReloadFileConfig
    {
        /// <summary>
        /// Reloads the configuration from the appsettings.json file.
        /// </summary>
        public IConfiguration ReloadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        /// <summary>
        /// Gets the configuration as a JSON string.
        /// </summary>
        /// <returns>JSON string representation of the configuration.</returns>
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