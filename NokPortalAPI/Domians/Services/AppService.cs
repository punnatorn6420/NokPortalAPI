using Newtonsoft.Json;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Domians.Services;
using NokPortal.Shared.DB;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domains.Services
{
    public class AppService : IAppService
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IConfiguration configuration;
        private readonly IAppRepository appRepository;
        private readonly IJwtService jwtService;
        private readonly HttpClient httpClient;
        private readonly IAppEnvRepository appEnvRepository;

        public AppService(IDbConnectionFactory connectionFactory, IConfiguration configuration, IAppRepository appRepository, IJwtService jwtService, HttpClient httpClient, IAppEnvRepository appEnvRepository)
        {
            this.connectionFactory = connectionFactory;
            this.appRepository = appRepository;
            this.configuration = configuration;
            this.jwtService = jwtService;

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            };
            this.httpClient = new HttpClient(httpClientHandler);
            this.appEnvRepository = appEnvRepository;
        }

        public async Task<bool> CreateAppAsync(RequestApp reqCreate)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            bool result = await appRepository.CreateAppAsync(connection, tran, reqCreate);
            if (result)
            {
                tran.Commit();
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAppAsync(RequestApp reqCreate)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            bool result = await appRepository.UpdateAppAsync(connection, tran, reqCreate);
            if (result)
            {
                tran.Commit();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ModelApp>> GetAllAppAsync()
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var apps = await appRepository.GetAllAppAsync(connection, tran);
                tran.Commit();
                return apps;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<ModelApp> GetAppByIdAsync(int appId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var app = await appRepository.GetAppByIdAsync(connection, tran, appId);
                tran.Commit();
                return app;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        /*
        public async Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

                var response = await httpClient.SendAsync(request);

                var app = await appRepository.GetAppByIdAsync(connection, tran, appId);
                tran.Commit();
                return new ResponseJwt { ExpiresTime = new DateTime(), Token = "asd" };
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
        */

        public async Task<IEnumerable<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env)
        {
            try
            {
                using var connection = connectionFactory.CreateConnection();
                connection.Open();
                using var tran = connection.BeginTransaction();
                try
                {
                    AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, env);
                    tran.Commit();

                    ResponseJwt jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtHourLimit);
                    string fullLinkAppRoles = appEnv.BaseURL += "/api/auth/roles";

                    var request = new HttpRequestMessage(HttpMethod.Get, fullLinkAppRoles);
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $" {jwtToken.Token}");

                    var response = await httpClient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var responseData = JsonConvert.DeserializeObject<YourResponseModel>(responseContent);
                    }
                    else
                    {
                        string errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Error: {errorContent}");
                    }
                    return null;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseAppLink> GetAppLink(int appId, int userId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var appEnv = await appEnvRepository.GetByIdAdminAsync(connection, tran, appId, userId, env);
                string fullLinkAppRedirect = appEnv.BaseURL += "/app_link?jwt=";
                tran.Commit();
                return new ResponseAppLink { AppLink = fullLinkAppRedirect };
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}