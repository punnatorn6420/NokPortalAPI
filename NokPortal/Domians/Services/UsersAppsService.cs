using NokPortal.Domains.Models;
using NokPortal.Domains.Repositories;
using NokPortal.Shared.DB;

namespace NokPortal.Domians.Services
{
    public class UsersAppsService : IUsersAppsService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly IUsersAppsRepository _usersAppsRepository;

        public UsersAppsService(IDbConnectionFactory connectionFactory, IUsersAppsRepository usersAppsRepository)
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
                int rowEffect = await _usersAppsRepository.AddUserAppAsync(connection, tran, userApp.AppID, userApp.UserID);
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
    }
}
