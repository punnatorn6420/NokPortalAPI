using System.Dynamic;
using NokCore.Api.JWT.Models;
using NokCore.Api.JWT.Services;
using NokCore.Identity.Models;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Shared.DB;

namespace NokPortalAPI.Domains.Services
{
    public class UserAppsService : IUserAppsService
    {
        private readonly DbConnectionFactory connectionFactory;
        private readonly IAssignedUsersRepositorys assignedUsersRepositiry;
        private readonly HttpClient httpClient;
        private readonly IAppEnvRepository appEnvRepository;
        private readonly IJwtService jwtService;
        private readonly IUserRepository userRepository;

        public UserAppsService(DbConnectionFactory connectionFactory, IAssignedUsersRepositorys assignedUsersRepositiry, HttpClient httpClient, IAppEnvRepository appEnvRepository, IJwtService jwtService, IUserRepository userRepository)
        {
            this.connectionFactory = connectionFactory;
            this.assignedUsersRepositiry = assignedUsersRepositiry;
            this.httpClient = httpClient;
            this.appEnvRepository = appEnvRepository;
            this.jwtService = jwtService;
            this.userRepository = userRepository;
        }

        /// <summary>
        /// Add relation between user and app with roles for each environment.
        /// </summary>
        /// <param name="appId">App id.</param>
        /// <param name="reqAddUserApp">Request to add user app.</param>
        /// <returns></returns>
        public async Task AddUserAppAsync(int appId, RequestAddUserApp reqAddUserApp)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                // Get app environment
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, appId, reqAddUserApp.Environment, tran);

                // Get user
                User user = await userRepository.GetUserByIdAsync(connection, reqAddUserApp.UserId, tran);

                // Add relation between user and app
                await assignedUsersRepositiry.CreateAssignedUsersAsync(connection, tran, appId, reqAddUserApp.UserId, reqAddUserApp.Roles, reqAddUserApp.Environment);

                tran.Commit();
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get user app info.
        /// </summary>
        /// <param name="appId">App id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>User app info.</returns>
        public async Task<ModelUserApp> GetUserAppAsync(int appId, int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await assignedUsersRepositiry.GetAssignedUserAsync(connection, tran, appId, userId);
                return userAppInfo;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Check role level.
        /// </summary>
        /// <param name="roleLevel">Role level.</param>
        /// <param name="appId">App id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>True if role level is valid, otherwise false.</returns>
        public async Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await assignedUsersRepositiry.GetAssignedUserAsync(connection, tran, appId, userId);

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

        /// <summary>
        /// Get all apps by user id.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>List of apps.</returns>
        public async Task<IEnumerable<App>> GetAllAppsByUserIdAsync(int userId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            IEnumerable<App> app = await assignedUsersRepositiry.GetAllAssingedUsersAppAsync(connection, tran, userId);

            return app;
        }

        /// <summary>
        /// Get JWT for target app.
        /// </summary>
        /// <param name="appId">App id.</param>
        /// <param name="env">App environment.</param>
        /// <returns></returns>
        public async Task<(AppEnv, JwtResponse)> GetJWTForTargetAppAsync(int appId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, appId, env, tran);
                JwtResponse jwtToken = jwtService.GenerateTokenForTargetApp(null, appEnv.SecretKey, appEnv.JwtHourLimit);
                tran.Commit();

                return (appEnv, jwtToken);
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<(AppEnv, JwtResponse)> GetJWTTokenTargetAppWithData(int appId, EnumEnvironmentType env, User user, IEnumerable<AppWithRoles> userAppsList)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                AppEnv appEnv = await appEnvRepository.GetByIdAsync(connection, appId, env, tran);

                var roles = userAppsList
                    .SelectMany(app => app.Roles)
                    .Distinct()
                    .ToList();

                dynamic jwtSetting = new ExpandoObject();
                jwtSetting.userID = user.UserId;
                jwtSetting.name = $"{user.FirstName} {user.LastName}";
                jwtSetting.objectId = user.ObjectId;
                jwtSetting.avatar = string.Empty;
                jwtSetting.email = user.Email;
                jwtSetting.company = string.Empty;
                jwtSetting.department = user.Department;
                jwtSetting.position = user.JobTitle;
                jwtSetting.roles = roles;

                JwtResponse jwtToken = jwtService.GenerateTokenForTargetAppWithData(jwtSetting, appEnv.SecretKey, appEnv.JwtHourLimit);
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
