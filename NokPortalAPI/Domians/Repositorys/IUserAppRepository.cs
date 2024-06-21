using System.Data;
using NokPortal.Domains.Models;

namespace NokPortal.Domains.Repositories
{
    public interface IUserAppRepository
    {
        Task<int> AddUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, int roleId);

        Task<int> DeleteUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<UserApp> GetUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<UserApp>> GetAllUserAppsAsync(IDbConnection conn, IDbTransaction tran);
    }
}
