using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppsEnvRepository
    {
        Task Create(IDbConnection conn, IDbTransaction tran, AppsEnvModel model);

        Task<AppsEnvModel> GetById(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<AppsEnvModel>> GetAll(IDbConnection conn, IDbTransaction tran);

        Task Update(IDbConnection conn, IDbTransaction tran, AppsEnvModel model);

        Task Delete(IDbConnection conn, IDbTransaction tran, int appId);
    }
}
