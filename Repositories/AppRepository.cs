using Microsoft.EntityFrameworkCore;
using NokAir.Core.Exceptions;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Shareds;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// Application repository.
    /// </summary>
    public class AppRepository : IAppRepository
    {
        private readonly AppDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppRepository"/> class.
        /// </summary>
        /// <param name="context"></param>
        public AppRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<App> AddAppAsync(App app)
        {
            // Ensure that the app name is unique.
            var existingApp = await context.Apps.FirstOrDefaultAsync(a => a.Name == app.Name.Trim());
            if (existingApp != null)
            {
                throw new DataValidationException(ErrorCode.E2001AppNameAlreadyExists, string.Empty);
            }

            var result = await context.Apps.AddAsync(app);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<int> RemoveAppByIdAsync(int id)
        {
            var app = await context.Apps.FindAsync(id);
            if (app == null)
            {
                return 0;
            }

            context.Apps.Remove(app);
            return await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<App?> FindAppByIdAsync(int id)
        {
            return await context.Apps.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<(IList<App> Items, int TotalRecords)> FindAppsByCriteriaAsync(AppSearchDto criteria)
        {
            IQueryable<App> query = context.Apps;
            var keyword = criteria.Keyword?.Trim();
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => (a.Name ?? string.Empty).Contains(keyword) || (a.Header ?? string.Empty).Contains(keyword));
            }
            var total = await query.CountAsync();
            var field = string.IsNullOrWhiteSpace(criteria.SortField) ? "Id" : criteria.SortField.Trim();
            var dir = criteria.Ascending ? "ascending" : "descending";
            query = query.OrderBy($"{field} {dir}");
            var skip = (Math.Max(1, criteria.PageNumber) - 1) * Math.Clamp(criteria.PageSize, 1, 200);
            var items = await query.Skip(skip).Take(Math.Clamp(criteria.PageSize, 1, 200)).ToListAsync();
            return (items, total);
        }

        /// <inheritdoc/>
        public async Task<IList<App>> FindAppsByUserIdAsync(int userId)
        {
            return await context.Apps
                .Where(a => a.UserAppRoleAssignments.Any(aua => aua.UserId == userId))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<int> UpdateAppAsync(App app)
        {
            context.Apps.Update(app);
            return await context.SaveChangesAsync();
        }
    }
}