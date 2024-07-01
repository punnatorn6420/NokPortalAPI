using NokCore.Identity.Models;
using NokPortal.Domains.Models;
using NokPortal.Domains.Repositories;
using NokPortal.Domians.Models;
using NokPortal.Shared.DB;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Services
{
    public class UserAppsService : IUserAppsService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IUserAppRepository _usersAppsRepository;

        public UserAppsService(IDbConnectionFactory connectionFactory, IUserAppRepository usersAppsRepository)
        {
            _connectionFactory = connectionFactory;
            _usersAppsRepository = usersAppsRepository;
        }

        public async Task<int> AddUserAppAsync(ModelUserApp userApp)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                int rowEffect = await _usersAppsRepository.AddUserAppAsync(connection, tran, userApp.AppId, userApp.UserId, (int)userApp.RoleId);
                if (rowEffect > 0)
                {
                    tran.Commit();
                }
                else
                {
                    tran.Rollback();
                }
                return rowEffect;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<ModelUserApp> GetUserAppAsync(int appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await _usersAppsRepository.GetUserAppAsync(connection, tran, appId, userId);
                return userAppInfo;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<bool> CheckRoleLevelAsync(EnumUserRole roleLevel, int appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await _usersAppsRepository.GetUserAppAsync(connection, tran, appId, userId);

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
                tran.Rollback();
                throw;
            }
        }

        public async Task<IEnumerable<ModelApp>> GetUserAllAppAsync(int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            IEnumerable<ModelApp> app = await _usersAppsRepository.GetUserAllAppAsync(connection, tran, userId);

            return app;
        }
    }
}
