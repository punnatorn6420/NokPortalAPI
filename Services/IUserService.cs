using NokAir.Core.Interfaces.Rbac.Services;
using NokPortalAPI.Dtos;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for user service.
    /// </summary>
    public interface IUserService<T> : IUserServiceBase<T> where T : class
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
        Task<ICollection<UserDto>> GetUsersByCriteriaAsync(UserSearchCriteriaDto searchCriteriaDto);


        Task<MyProfileDto?> GetMyProfileAsync(int userId);
    }
}