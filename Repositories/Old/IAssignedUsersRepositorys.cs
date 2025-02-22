using System.Data;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    public interface IAssignedUsersRepositorys
    {
        Task<ModelUserApp> GetAssignedUserAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId);

        Task<IEnumerable<App>> GetAllAssingedUsersAppAsync(IDbConnection conn, IDbTransaction tran, int userId);

        Task CreateAssignedUsersAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, IEnumerable<int> roles, EnumEnvironmentType environment);
    }
}
