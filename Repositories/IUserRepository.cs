using NokAir.Core.Interfaces.Rbac.Repositories;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for user repository.
    /// </summary>
    public interface IUserRepository<T> : IUserRepositoryBase<T> where T : class
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<T>> GetUsersByAppIdAsync(int appId);

        /// <summary>
        /// Gets list of users by search criteria.
        /// </summary>
        /// <param name="searchCriteria">User search criteria.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<User>> GetUsersByCriteriaAsync(UserSearchCriteria searchCriteria);
    }
}
