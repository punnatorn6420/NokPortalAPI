using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppEnvRepository
    {
        Task Create(IDbConnection conn, IDbTransaction tran, RequestCreateAppEnv model);

        Task<RequestCreateAppEnv> GetById(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<RequestCreateAppEnv>> GetAll(IDbConnection conn, IDbTransaction tran);

        Task Update(IDbConnection conn, IDbTransaction tran, RequestCreateAppEnv model);

        Task Delete(IDbConnection conn, IDbTransaction tran, int appId);
    }
}
