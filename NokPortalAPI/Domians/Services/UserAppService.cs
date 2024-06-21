using NokCore.Identity.Models;
using NokPortal.Domains.Models;
using NokPortal.Domains.Repositories;
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

        public async Task<int> AddUserAppAsync(UserApp userApp)
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
                return rowEffect;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }

        public async Task<UserApp> GetUserAppAsync(int appId, int userId)
        {
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            using var tran = connection.BeginTransaction();

            try
            {
                UserApp userAppInfo = await _usersAppsRepository.GetUserAppAsync(connection, tran, appId, userId);
                return userAppInfo;
            }
            catch (Exception)
            {
                tran.Rollback();
                throw;
            }
        }
    }
}
