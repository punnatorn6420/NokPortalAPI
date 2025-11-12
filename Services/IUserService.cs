using NokAir.Core.Interfaces.Rbac.Services;
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
        /// <returns>List of users.</returns>
        Task<ICollection<T>> GetUsersByAppIdAsync(int appId);

        /// <summary>
        /// Gets list of users by search criteria.
        /// </summary>
        /// <param name="searchCriteriaDto">User search criteria.</param>
        /// <returns>List of users.</returns>
        Task<(IList<UserDto> Items, int TotalRecords)> GetUsersByCriteriaAsync(UserSearchCriteriaDto searchCriteriaDto);

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<MyProfileDto?> GetMyProfileAsync(int userId);

        /// <summary>
        ///
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        Task UpdateUserRoleAsync(int userId, int roleId);
    }
}