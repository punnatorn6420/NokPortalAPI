using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppEnvRepository
    {
        Task Create(IDbConnection conn, IDbTransaction tran, AppEnv model);

        Task<IEnumerable<AppEnv>> GetByIdAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<AppEnv>> GetAll(IDbConnection conn, IDbTransaction tran);

        Task Update(IDbConnection conn, IDbTransaction tran, AppEnv model);

        Task Delete(IDbConnection conn, IDbTransaction tran, int appId);
    }
}
