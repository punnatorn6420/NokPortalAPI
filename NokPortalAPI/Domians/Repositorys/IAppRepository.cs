using System.Data;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IAppRepository
    {
        Task<bool> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqCreate);

        Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqUpdate);

        Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran);

        Task<IEnumerable<AppEnv>> GetAllAppEnvAsync(IDbConnection conn, IDbTransaction tran);

        Task<App> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, int id, string name);

        Task<bool> DeleteAppAsync(IDbConnection conn, IDbTransaction tran, int id);
    }
}
