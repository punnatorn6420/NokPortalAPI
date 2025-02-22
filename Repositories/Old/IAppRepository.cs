using System.Data;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    public interface IAppRepository
    {
        Task<bool> CreateAppAsync(IDbConnection conn, App app, IDbTransaction? tran = null);

        Task<bool> UpdateAppAsync(IDbConnection conn, App app, IDbTransaction? tran = null);

        Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction? tran = null);

        Task<App> GetAppByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null);

        Task<bool> DeleteAppByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null);
    }
}
