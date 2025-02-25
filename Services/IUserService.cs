using NokCore.Identity.Services;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for user service.
    /// </summary>
    public interface IUserService<T> : IBaseUserService<T>
    {
        /// <summary>
        /// Gets list of users by application ID.
        /// </summary>
        /// <param name="appId">Application ID.</param>
        /// <returns>List of users.</returns>
        Task<ICollection<T>> GetUsersByAppIdAsync(int appId);
    }
}
