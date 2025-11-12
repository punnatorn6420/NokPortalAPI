using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Core.Interfaces.Rbac.Repositories;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for role repository.
    /// </summary>
    public interface IRoleRepository<T> : IRoleRepositoryBase<T>
        where T : class
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="defaultRoleId"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task AssignDefaultRoleAsync(int userId, int defaultRoleId = 3);

        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<IRole>> GetAllRolesAsync();
    }
}