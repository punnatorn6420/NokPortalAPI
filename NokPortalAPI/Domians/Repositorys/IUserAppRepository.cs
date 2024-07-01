using System.Data;
using NokPortal.Domains.Models;
using NokPortal.Domians.Models;

namespace NokPortal.Domains.Repositories
{
    public interface IUserAppRepository
    {
        Task<int> AddUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, int roleId);

        Task<int> DeleteUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<ModelUserApp> GetUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<ModelUserApp>> GetAllUserAppsAsync(IDbConnection conn, IDbTransaction tran);

        Task<IEnumerable<ModelApp>> GetUserAllAppAsync(IDbConnection conn, IDbTransaction tran, int userId);
    }
}
