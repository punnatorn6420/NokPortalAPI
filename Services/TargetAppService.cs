using Newtonsoft.Json;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.CoreModels.ApiResponses;
using NokCore.Exceptions;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using NokPortalAPI.Repositories;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Target app service.
    /// </summary>
    public class TargetAppService : ITargetAppService
    {
        private readonly IJwtService jwtService;
        private readonly IAppRepository appRepository;
        private readonly HttpClient httpClient;

        public TargetAppService(HttpClient httpClient, AppRepository appRepository, IJwtService jwtService)
        {
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            this.httpClient = new HttpClient(httpClientHandler);
            this.appRepository = appRepository;
            this.jwtService = jwtService;
        }

        // <inheritdoc />
        public async Task<IList<Role>> GetAppRolesByAppIdAndEnvAsync(int appId, EnumEnvironmentType env)
        {
            try
            {
                var appEnv = await appRepository.GetAppEnvironmentByIdAndEnvAsync(appId, env);
                if (appEnv == null)
                {
                    throw new DataValidationException("App environment not found");
                }

                JwtResponse jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtExpirationHour);
                string fullLinkAppRoles = appEnv.BaseURL += "/roles";

                var request = new HttpRequestMessage(HttpMethod.Get, fullLinkAppRoles);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $" {jwtToken.Token}");

                var response = await httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var res = JsonConvert.DeserializeObject<BaseSuccessResponse<IList<Role>>>(responseContent) ?? throw new DataValidationException("Empty roles data");
                    return res.Data ?? throw new DataValidationException("Empty roles data");
                }
                else
                {
                    throw new Exception($"Error: {responseContent}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
