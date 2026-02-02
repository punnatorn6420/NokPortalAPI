using NokAir.Core.Abstractions.Repositories.Rbac;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for user repository.
    /// </summary>
    public interface IUserRepository<T> : IUserRepositoryBase<T>
        where T : class
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<T>> FindUsersByAppIdAsync(int appId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of users by search criteria.
        /// </summary>
        /// <param name="searchCriteria">User search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of users.</returns>
        Task<(IList<User> Items, int TotalRecords)> FindUsersByCriteriaAsync(UserSearchCriteria searchCriteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the profile of the current user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MyProfileDto?> FindMyProfileAsync(int userId, CancellationToken cancellationToken = default);
    }
}