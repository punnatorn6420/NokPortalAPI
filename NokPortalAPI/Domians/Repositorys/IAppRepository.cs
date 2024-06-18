using System.Data;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public interface IAppRepository
    {
        Task<int> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestCreateApp reqCreate);

        Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran);

        Task<App> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id);

        Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, int id, string name);

        Task<bool> DeleteAppAsync(IDbConnection conn, IDbTransaction tran, int id);
    }
}
