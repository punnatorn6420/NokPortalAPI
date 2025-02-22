using NokCore.Identity.Models;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for user service.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds a user.
        /// </summary>
        /// <param name="user">User to add.</param>
        /// <returns>Add user.</returns>
        Task<User> AddUserAsync(User user);

        /// <summary>
        /// Gets a user by ID.
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <returns>User object if found; otherwise, null.</returns>
        Task<User?> GetUserByIdAsync(int id);

        /// <summary>
        /// Gets a user by email.
        /// </summary>
        /// <param name="email">Email address.</param>
        /// <returns>User object if found; otherwise, null.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Gets list of users by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <returns>List of users.</returns>
        Task<IList<User>> GetUsersByCriteriaAsync(UserSearchCriteria searchCriteria);

        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of users.</returns>
        Task<IList<User>> GetUsersByAppIdAsync(int appId);

        /// <summary>
        /// Updates a user.
        /// </summary>
        /// <param name="user">User to update.</param>
        /// <returns>True if the user is updated; otherwise, false.</returns>
        Task<bool> UpdateUserAsync(User user);
    }
}
