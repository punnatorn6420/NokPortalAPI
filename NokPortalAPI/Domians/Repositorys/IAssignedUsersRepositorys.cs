using System.Data;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IAssignedUsersRepositorys
    {
        Task<ModelUserApp> GetUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<App>> GetUserAllAppAsync(IDbConnection conn, IDbTransaction tran, int userId);

        Task AssignedUsersAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, IEnumerable<int> roles);
    }
}
