using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Interface for application repository.
    /// </summary>
    public interface IAppRepository
    {
        /// <summary>
        /// Creates an application.
        /// </summary>
        /// <param name="app">Application to create.</param>
        /// <returns>Created application.</returns>
        Task<App> AddAppAsync(App app);

        /// <summary>
        /// Deletes an application by ID.
        /// </summary>
        /// <param name="id">Application ID.</param>
        /// <returns>Returns the number of rows affected.</returns>
        Task<int> DeleteAppByIdAsync(int id);

        /// <summary>
        /// Gets an application by ID.
        /// </summary>
        /// <param name="id">Application ID.</param>
        /// <returns>An application.</returns>
        Task<App?> GetAppByIdAsync(int id);

        /// <summary>
        /// Gets list of applications without environments by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <returns>List of applications.</returns>
        Task<IList<App>> GetAppsByCriteriaAsync(AppSearchCriteria searchCriteria);

        /// <summary>
        /// Gets list of applications with all the environments by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <returns>List of applications.</returns>
        Task<IList<App>> GetAppsByUserIdAsync(int userId);

        /// <summary>
        /// Updates an application.
        /// </summary>
        /// <param name="app">Application to update.</param>
        /// <returns>Returns the number of rows affected.</returns>
        Task<int> UpdateAppAsync(App app);
    }
}
