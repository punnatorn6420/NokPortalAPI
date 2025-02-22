using System.Data;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    /// <summary>
    /// Interface for application environment repository.
    /// </summary>
    public interface IAppEnvRepository
    {
        Task CreateAsync(IDbConnection conn, AppEnvironment appEnv, IDbTransaction? tran = null);

        Task<AppEnvironment> GetByIdForAdminAsync(IDbConnection conn, int appId, int userId, EnumEnvironmentType env, IDbTransaction? tran = null);

        Task<AppEnvironment> GetByIdAsync(IDbConnection conn, int appId, EnumEnvironmentType env, IDbTransaction? tran = null);

        Task<IEnumerable<AppEnvironment>> GetAllAsync(IDbConnection conn, IDbTransaction? tran = null);

        Task UpdateAsync(IDbConnection conn, AppEnvironment appEnv, IDbTransaction? tran = null);

        Task DeleteByAppIdAsync(IDbConnection conn, int appId, IDbTransaction? tran = null);
    }
}
