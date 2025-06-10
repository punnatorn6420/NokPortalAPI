using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Core.Interfaces.Rbac.Repositories;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for role repository.
    /// </summary>
    public interface IRoleRepository<T> : IRoleRepositoryBase<T> where T : class
    {
        Task AssignDefaultRoleAsync(int userId, int defaultRoleId = 3);

        Task<IEnumerable<IRole>> GetAllRolesAsync();
    }
}
