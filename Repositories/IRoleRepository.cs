using NokAir.Core.Interfaces.Rbac.Repositories;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for role repository.
    /// </summary>
    public interface IRoleRepository<T> : IRoleRepositoryBase<T> where T : class
    {
    }
}
