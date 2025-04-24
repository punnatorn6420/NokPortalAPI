using Microsoft.EntityFrameworkCore;
using NokAir.Core.Exceptions;
using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;
using NokPortalAPI.Shareds;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    public class AppRepository : IAppRepository
    {
        private readonly AppDbContext context;

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

            var result =await context.Apps.AddAsync(app);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteAppByIdAsync(int id)
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
        public async Task<App?> GetAppByIdAsync(int id)
        {
            return await context.Apps.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<IList<App>> GetAppsByCriteriaAsync(AppSearchDto searchCriteria)
        {
            IQueryable<App> query = context.Apps;

            var keyword = searchCriteria.Keyword?.Trim();
            if (!string.IsNullOrEmpty(keyword))
                if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(a => a.Name.Contains(keyword) || a.Header.Contains(keyword));
            }

            var sortField = !string.IsNullOrEmpty(searchCriteria.SortField) ? searchCriteria.SortField.Trim() : "Id";
            if (!string.IsNullOrEmpty(sortField))
            {
                var sortDirection = searchCriteria.Ascending ? "ascending" : "descending";
                query = query.OrderBy($"{sortField} {sortDirection}");
            }

            return await query
                .Skip((searchCriteria.PageNumber - 1) * searchCriteria.PageSize)
                .Take(searchCriteria.PageSize)
                .ToListAsync();
        }



        /// <inheritdoc/>
        public async Task<IList<App>> GetAppsByUserIdAsync(int userId)
        {
            return await context.Apps
                .Where(a => a.AssignedApps.Any(aua => aua.UserId == userId))
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
