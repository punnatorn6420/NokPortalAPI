using System.Data;
using NokPortal.Domains.Models;

namespace NokPortal.Domains.Repositories
{
    public interface IUsersAppsRepository
    {
        Task<int> AddUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<int> DeleteUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<UserApp> GetUserAppAsync(IDbConnection conn, int appId, int userId);

        Task<IEnumerable<UserApp>> GetAllUserAppsAsync(IDbConnection conn);
    }
}
