//using Newtonsoft.Json;
//using NokCore.Api.JWT.Models;
//using NokCore.Api.JWT.Services;
//using NokCore.Exceptions;
//using NokCore.Identity.Models;
//using NokPortalAPI.Models;
//using NokPortalAPI.Repositories.Old;
//using NokPortalAPI.Shareds.DB;

//namespace NokPortalAPI.Services.Old
//{
//    public class AppService : IAppService
//    {
//        private readonly DbConnectionFactory connectionFactory;
//        private readonly IConfiguration configuration;
//        private readonly IAppRepository appRepository;
//        private readonly IJwtService jwtService;
//        private readonly HttpClient httpClient;
//        private readonly IAppEnvRepository appEnvRepository;

//        public AppService(DbConnectionFactory connectionFactory, IConfiguration configuration, IAppRepository appRepository, IJwtService jwtService, HttpClient httpClient, IAppEnvRepository appEnvRepository)
//        {
//            this.connectionFactory = connectionFactory;
//            this.appRepository = appRepository;
//            this.configuration = configuration;
//            this.jwtService = jwtService;

//            var httpClientHandler = new HttpClientHandler
//            {
//                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
//            };
//            this.httpClient = new HttpClient(httpClientHandler);
//            this.appEnvRepository = appEnvRepository;
//        }

//        /// <summary>
//        /// Create application.
//        /// </summary>
//        /// <param name="app">Application.</param>
//        /// <returns>True if success, otherwise false.</returns>
//        public async Task<bool> CreateAppAsync(App app)
//        {
//            using var connection = await connectionFactory.CreateConnectionAsync();
//            connection.Open();
//            using var tran = connection.BeginTransaction();
//            bool result = await appRepository.CreateAppAsync(connection, app, tran);
//            if (result)
//            {
//                tran.Commit();
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Update application.
//        /// </summary>
//        /// <param name="app">Application.</param>
//        /// <returns>True if success, otherwise false.</returns>
//        public async Task<bool> UpdateAppAsync(App app)
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();
//            using var tran = connection.BeginTransaction();
//            bool result = await appRepository.UpdateAppAsync(connection, app, tran);
//            if (result)
//            {
//                tran.Commit();
//                return true;
//            }
//            return false;
//        }

//        /// <summary>
//        /// Get all applications.
//        /// </summary>
//        /// <returns>List of applications.</returns>
//        public async Task<IEnumerable<App>> GetAllAppsAsync()
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();

//            try
//            {
//                var apps = await appRepository.GetAllAppAsync(connection);
//                return apps;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public async Task<App> GetAppByIdAsync(int appId)
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();
//            using var tran = connection.BeginTransaction();
//            try
//            {
//                var app = await appRepository.GetAppByIdAsync(connection, appId, tran);
//                tran.Commit();
//                return app;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        /*
//        public async Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId)
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();
//            using var tran = connection.BeginTransaction();
//            try
//            {
//                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
//                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

//                var response = await httpClient.SendAsync(request);

//                var app = await appRepository.GetAppByIdAsync(connection, tran, appId);
//                tran.Commit();
//                return new ResponseJwt { ExpiresTime = new DateTime(), Token = "asd" };
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        */

//        public async Task<IEnumerable<Role>> GetAppTargetAllRole(int appId, EnumEnvironmentType env)
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();

//            try
//            {
//                AppEnvironment appEnv = await appEnvRepository.GetByIdAsync(connection, appId, env);

//                JwtResponse jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtExpirationHour);
//                string fullLinkAppRoles = appEnv.BaseURL += "/roles";

//                var request = new HttpRequestMessage(HttpMethod.Get, fullLinkAppRoles);
//                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $" {jwtToken.Token}");

//                var response = await httpClient.SendAsync(request);
//                string responseContent = await response.Content.ReadAsStringAsync();
//                if (response.IsSuccessStatusCode)
//                {
//                    ResponseRolesTargetApp roles = JsonConvert.DeserializeObject<ResponseRolesTargetApp>(responseContent) ?? throw new DataValidationException("Empty roles data");
//                    return roles.Data;
//                }
//                else
//                {
//                    throw new Exception($"Error: {responseContent}");
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public async Task<ResponseAppLink> GetAppLink(int appId, int userId, EnumEnvironmentType env)
//        {
//            using var connection = connectionFactory.CreateConnection();
//            connection.Open();

//            try
//            {
//                var appEnv = await appEnvRepository.GetByIdForAdminAsync(connection, appId, userId, env);
//                string fullLinkAppRedirect = appEnv.BaseURL += "/app_link?jwt=";
//                return new ResponseAppLink { AppLink = fullLinkAppRedirect };
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//    }
//}