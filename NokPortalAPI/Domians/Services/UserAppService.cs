using System.Text;
using System.Text.Json.Nodes;
using Newtonsoft.Json;
using NokCore.Api.Controllers;
using NokCore.Api.JwtToken.Models;
using NokCore.Api.JwtToken.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Domians.Models;
using NokPortalAPI.Shared.DB;

namespace NokPortalAPI.Domains.Services
{
    public class UserAppsService : IUserAppsService
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IAssignedUsersRepositorys assignedUsersRepositiry;
        private readonly HttpClient httpClient;
        private readonly IAppEnvRepository appEnvRepository;
        private readonly IJwtService jwtService;
        private readonly IUserRepository userRepository;

        public UserAppsService(IDbConnectionFactory connectionFactory, IAssignedUsersRepositorys assignedUsersRepositiry, HttpClient httpClient, IAppEnvRepository appEnvRepository, IJwtService jwtService, IUserRepository userRepository)
        {
            this.connectionFactory = connectionFactory;
            this.assignedUsersRepositiry = assignedUsersRepositiry;
            this.httpClient = httpClient;
            this.appEnvRepository = appEnvRepository;
            this.jwtService = jwtService;
            this.userRepository = userRepository;
        }

        public async Task AddUserAppAsync(int appId, RequestAddUserApp addUser)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, addUser.Environment);
                User user = await userRepository.GetUserByIdAsync(connection, tran, addUser.UserId);
                tran.Commit();

                ResponseJwt jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtHourLimit);
                string fullLinkAppRoles = $"{appEnv.BaseURL}/assign-user";

                var assignedUser = new RequestAssignedUser
                {
                    Name = $"{user.FirstName} {user.LastName}",
                    Email = user.Email,
                    Roles = addUser.Roles,
                    Department = user.Department,
                    Position = string.Empty,
                };

                var json = JsonConvert.SerializeObject(assignedUser);

                var request = new HttpRequestMessage(HttpMethod.Post, fullLinkAppRoles);
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken.Token);
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await httpClient.SendAsync(request);
                string responseContent = await response.Content.ReadAsStringAsync();
                ApiAppResponse jsonResponse = JsonConvert.DeserializeObject<ApiAppResponse>(responseContent) ?? throw new InvalidOperationException("Deserialization resulted in null");

                if (response.IsSuccessStatusCode)
                {
                    var responseData = JsonConvert.DeserializeObject(responseContent);
                }
                else if (jsonResponse.Message == "Email already exists")
                {
                    Console.WriteLine($"Error: {responseContent}");
                }
                else
                {
                    throw new Exception($"Error: {responseContent}");
                }

                await assignedUsersRepositiry.AssignedUsersAsync(connection, tran, appId, addUser.UserId, addUser.Roles);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ModelUserApp> GetUserAppAsync(int appId, int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await assignedUsersRepositiry.GetUserAppAsync(connection, tran, appId, userId);
                return userAppInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await assignedUsersRepositiry.GetUserAppAsync(connection, tran, appId, userId);

                if ((int)userAppInfo.RoleId >= (int)roleLevel)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<App>> GetUserAllAppAsync(int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            IEnumerable<App> app = await assignedUsersRepositiry.GetUserAllAppAsync(connection, tran, userId);

            return app;
        }
    }
}
