using System.Data;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;
using NokPortal.Shared.DB;

namespace NokPortal.Domians.Services
{
    public class AppsEnvService : IAppsEnvService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IAppsEnvRepository _appsEnvRepository;
        private readonly IAppRepository _appRepository;

        public AppsEnvService(IDbConnectionFactory connectionFactory, IAppsEnvRepository appsEnvRepository, IAppRepository appRepository)
        {
            _connectionFactory = connectionFactory;
            _appsEnvRepository = appsEnvRepository;
            _appRepository = appRepository;
        }

        public async Task<bool> CreateAppsEnv(AppsEnvModel model)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                App app = await _appRepository.GetAppByIdAsync(connection, tran, model.AppID);
                if (app != null)
                {
                    var appsEnvModel = new AppsEnvModel()
                    {
                        AppID = app.AppID,
                        Environment = model.Environment,
                        BaseURL = model.BaseURL,
                        Additional = model.Additional,
                        SecretKey = model.SecretKey,
                        JwtHourLimit = model.JwtHourLimit,
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

        public async Task<AppsEnvModel> GetById(RequestAppInfo appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                AppsEnvModel appEnv = await _appsEnvRepository.GetById(connection, tran, appId.AppId, userId);
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
