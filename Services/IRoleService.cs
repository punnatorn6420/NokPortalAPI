using NokAir.Core.Abstractions.Entities.Rbac;
using NokAir.Core.Abstractions.Services.Rbac;

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