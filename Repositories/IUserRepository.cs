using NokCore.Identity.Repositories;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for user repository.
    /// </summary>
    public interface IUserRepository<T> : IBaseUserRepository<T>
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<T>> GetUsersByAppIdAsync(int appId);
    }
}
