using System.Data;
using Newtonsoft.Json;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Shared.DB;

namespace NokPortalAPI.Domains.Services
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

        public async Task<IEnumerable<App>> GetAllAppAsync()
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
                throw;
            }
        }


        public async Task<IEnumerable<AppEnv>> GetAllAppEnvAsync()
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var apps = await appRepository.GetAllAppEnvAsync(connection, tran);
                tran.Commit();
                return apps;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<App> GetAppByIdAsync(int appId)
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
                throw;
            }
        }
        */

        public async Task<IEnumerable<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, env);
                tran.Commit();

                ResponseJwt jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtHourLimit);
                string fullLinkAppRoles = appEnv.BaseURL += "/roles";

                var request = new HttpRequestMessage(HttpMethod.Get, fullLinkAppRoles);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $" {jwtToken.Token}");

                var response = await httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    ResponseRolesTargetApp roles = JsonConvert.DeserializeObject<ResponseRolesTargetApp>(responseContent) ?? throw new DataException("Empty roles data");
                    return roles.Data;
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