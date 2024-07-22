using System.Text;
using Newtonsoft.Json;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Shared.DB;
using System.Data;
using System.Dynamic;

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

                await assignedUsersRepositiry.AssignedUsersAsync(connection, tran, appId, addUser.UserId, addUser.Roles, addUser.Environment);
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

        public async Task<(AppEnv, ResponseJwt)> GetJWTTokenTargetApp(int appId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, env);
                ResponseJwt jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtHourLimit);
                Console.WriteLine("appEnv", appEnv, "jwtToken", jwtToken);

                tran.Commit();

                return (appEnv, jwtToken);
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<(AppEnv, ResponseJwt)> GetJWTTokenTargetAppWithData(int appId, EnumEnvironmentType env, User user, IEnumerable<UserApps> userAppsList)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, env);

                var roles = userAppsList
                    .SelectMany(userApps => userApps.App)
                    .SelectMany(app => app.Roles)
                    .Distinct()
                    .ToList();

                dynamic jwtSetting = new ExpandoObject();
                jwtSetting.userID = user.UserId;
                jwtSetting.name = $"{user.FirstName} {user.LastName}";
                jwtSetting.avatar = "";
                jwtSetting.email = user.Email;
                jwtSetting.company = "";
                jwtSetting.department = user.Department;
                jwtSetting.position = user.JobTitle;
                jwtSetting.roles = roles;

                ResponseJwt jwtToken = jwtService.GenerateTokenForTargetAppWithData(jwtSetting, appEnv.SecretKey, appEnv.JwtHourLimit);
                tran.Commit();

                return (appEnv, jwtToken);
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

    }
}
