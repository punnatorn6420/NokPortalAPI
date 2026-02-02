using Newtonsoft.Json.Serialization;
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
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Create app.</returns>
        Task<AppDto> AddAppAsync(AppDto app, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an app by ID asynchronously.
        /// </summary>
        /// <param name="id">App ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the app is deleted; otherwise, false.</returns>
        Task<bool> DeleteAppByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an app by ID asynchronously.
        /// </summary>
        /// <param name="id">App ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Application object if found; otherwise, null.</returns>
        Task<AppDto?> GetAppByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of applications without environments by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of applications.</returns>
        Task<(IList<AppDto> Items, int TotalRecords)> GetAppsByCriteriaAsync(AppSearchDto searchCriteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an app asynchronously.
        /// </summary>
        /// <param name="app">App to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if the app is updated; otherwise, false.</returns>
        Task<bool> UpdateAppAsync(AppDto app, CancellationToken cancellationToken = default);
    }
}