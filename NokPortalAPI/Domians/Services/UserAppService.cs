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
                tran.Commit();

                return (appEnv, jwtToken);
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<(AppEnv, ResponseJwt)> GetJWTTokenTargetAppWithData(int appId, EnumEnvironmentType env, User user, IEnumerable<AppWithRoles> userAppsList)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, tran, appId, env);

                var roles = userAppsList
                    .SelectMany(app => app.Roles)
                    .Distinct()
                    .ToList();

                dynamic jwtSetting = new ExpandoObject();
                jwtSetting.userID = user.UserId;
                jwtSetting.name = $"{user.FirstName} {user.LastName}";
                jwtSetting.objectId = user.ObjectId;
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
