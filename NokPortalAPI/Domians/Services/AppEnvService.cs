using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Shared.DB;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Services
{
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

        public async Task<bool> CreateAppsEnv(AppEnv reqCreateAppEnv, int appId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelApp app = await appRepository.GetAppByIdAsync(connection, tran, appId);
                if (app != null)
                {
                    var appsEnvModel = new AppEnv()
                    {
                        AppId = appId,
                        Environment = reqCreateAppEnv.Environment,
                        BaseURL = reqCreateAppEnv.BaseURL,
                        Additional = reqCreateAppEnv.Additional,
                        SecretKey = reqCreateAppEnv.SecretKey,
                        JwtHourLimit = reqCreateAppEnv.JwtHourLimit,
                    };
                    await appsEnvRepository.CreateAsync(connection, tran, appsEnvModel);
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

        public async Task<bool> UpdateAppsEnv(AppEnv reqCreateAppEnv, int appId)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelApp app = await appRepository.GetAppByIdAsync(connection, tran, appId);
                if (app != null)
                {
                    var appsEnvModel = new AppEnv()
                    {
                        AppId = appId,
                        Environment = reqCreateAppEnv.Environment,
                        BaseURL = reqCreateAppEnv.BaseURL,
                        Additional = reqCreateAppEnv.Additional,
                        SecretKey = reqCreateAppEnv.SecretKey,
                        JwtHourLimit = reqCreateAppEnv.JwtHourLimit,
                    };
                    await appsEnvRepository.UpdateAsync(connection, tran, appsEnvModel);
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

        public async Task<AppEnv> GetById(int appId, int userId, EnumEnvironmentType env)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                AppEnv appEnv = await appsEnvRepository.GetByIdAdminAsync(connection, tran, appId, userId, env);
                if (appEnv != null)
                {
                    tran.Commit();
                    return appEnv;
                }
                throw new Exception("appEnv null value");
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
