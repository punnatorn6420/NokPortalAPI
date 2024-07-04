using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppRepository
    {
        Task<bool> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqCreate);

        Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqUpdate);

        Task<IEnumerable<ModelApp>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran);

        Task<ModelApp> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, int id, string name);

        Task<bool> DeleteAppAsync(IDbConnection conn, IDbTransaction tran, int id);
    }
}
