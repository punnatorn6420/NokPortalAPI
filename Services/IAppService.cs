using NokPortalAPI.Dtos;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// Interface for the AppService.
    /// </summary>
    public interface IAppService
    {
        /// <summary>
        /// Creates an app asynchronously.
        /// </summary>
        /// <param name="app">The app to create.</param>
        /// <returns>Create app.</returns>
        Task<AppDto> AddAppAsync(AppDto app);

        /// <summary>
        /// Deletes an app by ID asynchronously.
        /// </summary>
        /// <param name="id">App ID.</param>
        /// <returns>True if the app is deleted; otherwise, false.</returns>
        Task<bool> DeleteAppByIdAsync(int id);

        /// <summary>
        /// Gets an app by ID asynchronously.
        /// </summary>
        /// <param name="id">App ID.</param>
        /// <returns>Application object if found; otherwise, null.</returns>
        Task<AppDto?> GetAppByIdAsync(int id);

        /// <summary>
        /// Gets list of applications without environments by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <returns>List of applications.</returns>
        Task<IList<AppDto>> GetAppsByCriteriaAsync(AppSearchDto searchCriteria);

        /// <summary>
        /// Updates an app asynchronously.
        /// </summary>
        /// <param name="app">App to update.</param>
        /// <returns>True if the app is updated; otherwise, false.</returns>
        Task<bool> UpdateAppAsync(AppDto app);
    }
}
