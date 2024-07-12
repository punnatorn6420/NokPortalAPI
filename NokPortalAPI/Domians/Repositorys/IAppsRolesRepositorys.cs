using NokPortalAPI.Domians.Models;

namespace NokPortalAPI.Domians.Repositorys
{
    public interface IAppsRolesRepositorys
    {
        Task<IEnumerable<AppsRole>> GetAllAsync();

        Task<AppsRole> GetByAssignedUserIdAsync(int assignedUserId);

        Task<int> AddAsync(AppsRole appsRole);

        Task<int> DeleteAsync(int assignedUserId, int roleId);
    }
}
