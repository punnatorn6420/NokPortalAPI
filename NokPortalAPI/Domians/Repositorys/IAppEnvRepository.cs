using System.Data;
using NokPortal.Domians.Models;
using NokPortalAPI.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppEnvRepository
    {
        Task CreateAsync(IDbConnection conn, IDbTransaction tran, AppEnv model);

        Task<AppEnv> GetByIdAdminAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, EnumEnvironmentType env);

        Task<AppEnv> GetByIdAsync(IDbConnection conn, IDbTransaction tran, int appId, EnumEnvironmentType env);

        Task<IEnumerable<AppEnv>> GetAllAsync(IDbConnection conn, IDbTransaction tran);

        Task UpdateAsync(IDbConnection conn, IDbTransaction tran, AppEnv model);

        Task DeleteAsync(IDbConnection conn, IDbTransaction tran, int appId);
    }
}
