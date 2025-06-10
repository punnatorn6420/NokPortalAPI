using NokAir.Core.Interfaces.Rbac.Entities;
using NokAir.Core.Interfaces.Rbac.Services;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Role service interface.
    /// </summary>
    public interface IRoleService : IRoleServiceBase
    {
        Task<IEnumerable<IRole>> GetAllRolesAsync();
    }

}
