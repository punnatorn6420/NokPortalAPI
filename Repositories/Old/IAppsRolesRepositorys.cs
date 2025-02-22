using NokPortalAPI.Models;
using System.Data;

namespace NokPortalAPI.Repositories.Old
{
    public interface IAppsRolesRepositorys
    {
        Task<IEnumerable<AppsRole>> GetAllAsync();

        Task<AppsRole> GetByAssignedUserIdAsync(int assignedUserId);

        Task<int> AddAsync(AppsRole appsRole);

        Task<int> DeleteAsync(int assignedUserId, int roleId);

        Task<IEnumerable<AppWithRoles>> GetAppsWithRolesByUserIdAsync(IDbConnection conn, int userId, EnumEnvironmentType env, IDbTransaction? tran = null);
    }
}
