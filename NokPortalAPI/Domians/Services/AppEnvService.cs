using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;
using NokPortalAPI.Shared.DB;

namespace NokPortalAPI.Domains.Services
{
    /// <summary>
    /// Service for managing application environment data.
    /// </summary>
    public class AppEnvService : IAppEnvService
    {
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IAppEnvRepository appsEnvRepository;
        private readonly IAppRepository appRepository;

        public AppEnvService(IDbConnectionFactory connectionFactory, IAppEnvRepository appsEnvRepository, IAppRepository appRepository)
        {
            this.connectionFactory = connectionFactory;
            this.appsEnvRepository = appsEnvRepository;
            this.appRepository = appRepository;
        }

        /// <summary>
        /// Create application environment.
        /// </summary>
        /// <param name="appEnv">Application environment.</param>
        /// <returns>True if success, otherwise false.</returns>
        public async Task<bool> CreateAppEnvAsync(AppEnv appEnv)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                App app = await appRepository.GetAppByIdAsync(connection, appEnv.AppId, tran);
                if (app != null)
                {
                    await appsEnvRepository.CreateAsync(connection, appEnv, tran);
                    tran.Commit();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        /// <summary>
        /// Update application environment.
        /// </summary>
        /// <param name="appEnv">Application environment.</param>
        /// <param name="appId">Application id.</param>
        /// <returns>True if success, otherwise false.</returns>
        public async Task<bool> UpdateAppEnvAsync(AppEnv appEnv)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                // Check if app exists
                App app = await appRepository.GetAppByIdAsync(connection, appEnv.AppId, tran);
                if (app != null)
                {
                    await appsEnvRepository.UpdateAsync(connection, appEnv, tran);
                    tran.Commit();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get all application environments.
        /// </summary>
        /// <returns>List of application environments.</returns>
        public async Task<IEnumerable<AppEnv>> GetAllAppEnvAsync()
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();

            try
            {
                return await appsEnvRepository.GetAllAsync(connection);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Delete application environment.
        /// </summary>
        /// <param name="appId">Application id.</param>
        /// <param name="env">Environment type.</param>
        /// <returns>AppEnv object.</returns>
        public async Task<AppEnv?> GetById(int appId, int userId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();

            try
            {
                return await appsEnvRepository.GetByIdForAdminAsync(connection, appId, userId, env);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
