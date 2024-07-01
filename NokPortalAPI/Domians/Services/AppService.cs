using System.Data;
using System.Text.Json;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Domians.Services;
using NokPortal.Shared.DB;

namespace NokPortal.Domains.Services
{
    public class AppService : IAppService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IConfiguration _configuration;
        private readonly IAppRepository _appRepository;
        private readonly IJwtService _jwtService;
        private readonly HttpClient _httpClient;

        public AppService(IDbConnectionFactory connectionFactory, IConfiguration configuration, IAppRepository appRepository, IJwtService jwtService, HttpClient httpClient)
        {
            _connectionFactory = connectionFactory;
            _appRepository = appRepository;
            _configuration = configuration;
            _jwtService = jwtService;
            _httpClient = httpClient;
        }

        public async Task<bool> CreateAppAsync(RequestCreateApp reqCreate)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            bool result = await _appRepository.CreateAppAsync(connection, tran, reqCreate);
            if (result)
            {
                tran.Commit();
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ModelApp>> GetAllAppAsync()
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var apps = await _appRepository.GetAllAppAsync(connection, tran);
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
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var app = await _appRepository.GetAppByIdAsync(connection, tran, appId);
                tran.Commit();
                return app;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<ResponseJwt> AssignUserTargetAppAsync(int appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "token");

                var response = await _httpClient.SendAsync(request);

                var app = await _appRepository.GetAppByIdAsync(connection, tran, appId);
                tran.Commit();
                return new ResponseJwt { ExpiresTime = new DateTime() , Token = "asd" };
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<Role>> GetAppTargetAllRole(int appId)
        {
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

                IEnumerable<Role> roles = JsonSerializer.Deserialize<IEnumerable<Role>>(json);
                return roles;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
