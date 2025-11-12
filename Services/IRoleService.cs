using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Core.Interfaces.Rbac.Services;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Role service interface.
    /// </summary>
    public interface IRoleService : IRoleServiceBase
    {
        /// <summary>
        ///
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<IEnumerable<IRole>> GetAllRolesAsync();
    }
}