using System.Data;
using NokPortal.Domains.Repositories;
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

        public AppService(IDbConnectionFactory connectionFactory, IConfiguration configuration, IAppRepository appRepository)
        {
            _connectionFactory = connectionFactory;
            _appRepository = appRepository;
            _configuration = configuration;
        }

        public async Task CreateAppAsync(RequestCreateApp reqCreate)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();
            try
            {
                int appId = await _appRepository.CreateAppAsync(connection, tran, reqCreate);
                if (appId > 0)
                {
                    tran.Commit();
                }
                else
                {
                    throw new Exception("No recode create.");
                }
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<App>> GetAllAppAsync()
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

        public async Task<App> GetAppByIdAsync(int appId)
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
    }
}
