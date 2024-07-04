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
        private readonly IDbConnectionFactory connectionFactory;
        private readonly IUserAppRepository usersAppsRepository;

        public UserAppsService(IDbConnectionFactory connectionFactory, IUserAppRepository usersAppsRepository)
        {
            this.connectionFactory = connectionFactory;
            this.usersAppsRepository = usersAppsRepository;
        }

        public async Task<int> AddUserAppAsync(ModelUserApp userApp)
        {
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                int rowEffect = await usersAppsRepository.AddUserAppAsync(connection, tran, userApp.AppId, userApp.UserId, (int)userApp.RoleId);
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
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await usersAppsRepository.GetUserAppAsync(connection, tran, appId, userId);
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
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                ModelUserApp userAppInfo = await usersAppsRepository.GetUserAppAsync(connection, tran, appId, userId);

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
            using var connection = connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            IEnumerable<ModelApp> app = await usersAppsRepository.GetUserAllAppAsync(connection, tran, userId);

            return app;
        }
    }
}
