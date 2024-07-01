using System.Data;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Shared.DB;

namespace NokPortal.Domians.Services
{
    public class AppEnvService : IAppEnvService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppEnvRepository _appsEnvRepository;
        private readonly IAppRepository _appRepository;

        public AppEnvService(IDbConnectionFactory connectionFactory, IAppEnvRepository appsEnvRepository, IAppRepository appRepository)
        {
            _connectionFactory = connectionFactory;
            _appsEnvRepository = appsEnvRepository;
            _appRepository = appRepository;
        }

        public async Task<bool> CreateAppsEnv(AppEnv reqCreateAppEnv, int appId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelApp app = await _appRepository.GetAppByIdAsync(connection, tran, appId);
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
                    await _appsEnvRepository.Create(connection, tran, appsEnvModel);
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

        public async Task<IEnumerable<AppEnv>> GetById(int appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                IEnumerable<AppEnv> appEnv = await _appsEnvRepository.GetByIdAsync(connection, tran, appId, userId);
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
