using NokAir.Core.Abstractions.Services.Rbac;
using NokPortalAPI.Dtos;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for user service.
    /// </summary>
    public interface IUserService<T> : IUserServiceBase<T>
        where T : class
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<T>> GetUsersByAppIdAsync(int appId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of users by search criteria.
        /// </summary>
        /// <param name="searchCriteriaDto">User search criteria.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>List of users.</returns>
        Task<(IList<UserDto> Items, int TotalRecords)> GetUsersByCriteriaAsync(UserSearchCriteriaDto searchCriteriaDto, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the profile of the current user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MyProfileDto?> GetMyProfileAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates the role of a user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateUserRoleAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    }
}