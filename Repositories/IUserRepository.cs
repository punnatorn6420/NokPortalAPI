using NokCore.Identity.Models;
using NokCore.Identity.Repositories;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for user repository.
    /// </summary>
    public interface IUserRepository : IBaseUserRepository
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of users.</returns>
        Task<IList<User>> GetUsersByAppIdAsync(int appId);
    }
}
