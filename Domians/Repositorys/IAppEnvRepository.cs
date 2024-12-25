using System.Data;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IAppEnvRepository
    {
        Task CreateAsync(IDbConnection conn, AppEnv appEnv, IDbTransaction? tran = null);

        Task<AppEnv> GetByIdForAdminAsync(IDbConnection conn, int appId, int userId, EnumEnvironmentType env, IDbTransaction? tran = null);

        Task<AppEnv> GetByIdAsync(IDbConnection conn, int appId, EnumEnvironmentType env, IDbTransaction? tran = null);

        Task<IEnumerable<AppEnv>> GetAllAsync(IDbConnection conn, IDbTransaction? tran = null);

        Task UpdateAsync(IDbConnection conn, AppEnv appEnv, IDbTransaction? tran = null);

        Task DeleteByAppIdAsync(IDbConnection conn, int appId, IDbTransaction? tran = null);
    }
}
