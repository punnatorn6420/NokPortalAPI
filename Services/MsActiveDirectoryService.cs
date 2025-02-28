using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NokCore.Exceptions;
using NokPortalAPI.Models;

namespace NokPortalAPI.Services
{
    public class MsActiveDirectoryService
    {
        private readonly ServiceSettings serviceSettings;
        private readonly HttpClient httpClient = new HttpClient();

        public MsActiveDirectoryService(IOptions<ServiceSettings> options)
        {
            this.serviceSettings = options.Value;
        }

        /// <summary>
        /// Generates the authorization URL for signing up with Microsoft Graph.
        /// </summary>
        /// <returns>The authorization URL.</returns>
        /// <exception cref="InvalidConfigurationException"></exception>
        public async Task<string> GenerateAuthorizationUrlSignInAsync()
        {
            var clientId = serviceSettings.OAuth2.ClientId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var responseType = serviceSettings.OAuth2.ResponseType ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var redirectUri = serviceSettings.OAuth2.RedirectUriSignIn ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var scope = serviceSettings.OAuth2.Scope ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var state = serviceSettings.OAuth2.State ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var tenantId = serviceSettings.OAuth2.TenantId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";
            return await Task.FromResult(authorizationUrl);
        }

        /// <summary>
        /// Generates the authorization URL for signing up with Microsoft Graph.
        /// </summary>
        /// <returns>The authorization URL.</returns>
        /// <exception cref="InvalidConfigurationException"></exception>
        public async Task<string> GenerateAuthorizationUrlSignUpAsync()
        {
            var clientId = serviceSettings.OAuth2.ClientId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var responseType = serviceSettings.OAuth2.ResponseType ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var redirectUri = serviceSettings.OAuth2.RedirectUriSignUp ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var scope = serviceSettings.OAuth2.Scope ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var state = serviceSettings.OAuth2.State ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var tenantId = serviceSettings.OAuth2.TenantId ?? throw new InvalidConfigurationException("OAuth2 settings are missing");
            var authorizationEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize";
            var authorizationUrl = $"{authorizationEndpoint}?client_id={Uri.EscapeDataString(clientId)}&response_type={Uri.EscapeDataString(responseType)}&redirect_uri={Uri.EscapeDataString(redirectUri)}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";
            return await Task.FromResult(authorizationUrl);
        }

        /// <summary>
        /// Gets the Microsoft user info using the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>Microsoft user info if successful, otherwise null.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<MicrosoftUserInfo?> GetMicrosoftUserInfoByTokenAsync(string token)
        {
            // Request to microsoft get AD info
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.SendAsync(request);
            MicrosoftUserInfo? msUser = null;

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                msUser = JsonConvert.DeserializeObject<MicrosoftUserInfo>(content);
            }
            else
            {
                throw new Exception("Error getting user info from Microsoft Graph");
            }

            return msUser;
        }
    }
}
