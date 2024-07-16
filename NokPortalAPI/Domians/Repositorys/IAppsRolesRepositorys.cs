using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public interface IAppsRolesRepositorys
    {
        Task<IEnumerable<AppsRole>> GetAllAsync();

        Task<AppsRole> GetByAssignedUserIdAsync(int assignedUserId);

        Task<int> AddAsync(AppsRole appsRole);

        Task<int> DeleteAsync(int assignedUserId, int roleId);
    }
}
