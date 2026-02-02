using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;

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
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Created application.</returns>
        Task<App> AddAppAsync(App app, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an application by ID.
        /// </summary>
        /// <param name="id">Application ID.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Returns the number of rows affected.</returns>
        Task<int> RemoveAppByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets an application by ID.
        /// </summary>
        /// <param name="id">Application ID.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>An application.</returns>
        Task<App?> FindAppByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of applications without environments by search criteria.
        /// </summary>
        /// <param name="searchCriteria">Search criteria.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>List of applications.</returns>
        Task<(IList<App> Items, int TotalRecords)> FindAppsByCriteriaAsync(AppSearchDto searchCriteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets list of applications with all the environments by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>List of applications.</returns>
        Task<IList<App>> FindAppsByUserIdAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an application.
        /// </summary>
        /// <param name="app">Application to update.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Returns the number of rows affected.</returns>
        Task<int> UpdateAppAsync(App app, CancellationToken cancellationToken = default);
    }
}