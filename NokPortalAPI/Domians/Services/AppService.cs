using Newtonsoft.Json;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Domains.Services;
using NokPortalAPI.Shared.DB;
using NokPortalAPI.Domains.Models;

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
            /*
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
            */

            try
            {
                string json = @"
                [
                    {
                        ""RoleId"": 1,
                        ""RoleName"": ""Admin"",
                        ""Permissions"": [
                            {
                                ""PermissionId"": 101,
                                ""PermissionName"": ""CreateCreditNote""
                            },
                            {
                                ""PermissionId"": 102,
                                ""PermissionName"": ""CancelEtaxInvoice""
                            }
                        ],
                        ""Active"": true,
                        ""CreatedAt"": ""2024-06-26T12:00:00Z"",
                        ""ModifiedAt"": ""2024-06-26T12:00:00Z""
                    },
                    {
                        ""RoleId"": 2,
                        ""RoleName"": ""User"",
                        ""Permissions"": [
                            {
                                ""PermissionId"": 103,
                                ""PermissionName"": ""ViewDashboard""
                            },
                            {
                                ""PermissionId"": 104,
                                ""PermissionName"": ""EditProfile""
                            }
                        ],
                        ""Active"": true,
                        ""CreatedAt"": ""2024-06-25T11:00:00Z"",
                        ""ModifiedAt"": ""2024-06-25T11:00:00Z""
                    },
                    {
                        ""RoleId"": 3,
                        ""RoleName"": ""Guest"",
                        ""Permissions"": [
                            {
                                ""PermissionId"": 105,
                                ""PermissionName"": ""ViewPublicContent""
                            }
                        ],
                        ""Active"": false,
                        ""CreatedAt"": ""2024-06-24T10:00:00Z"",
                        ""ModifiedAt"": ""2024-06-24T10:00:00Z""
                    }
                ]";

                IEnumerable<Role> roles = JsonConvert.DeserializeObject<IEnumerable<Role>>(json);

                return roles;
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