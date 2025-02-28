using Microsoft.EntityFrameworkCore;
using NokPortalAPI.Models;
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
        public async Task<IList<App>> GetAppsByCriteriaAsync(AppSearchCriteria searchCriteria)
        {
            IQueryable<App> query = context.Apps;

            if (!string.IsNullOrEmpty(searchCriteria.Keyword))
            {
                query = query.Where(a => a.Name.Contains(searchCriteria.Keyword) || a.Header.Contains(searchCriteria.Keyword));
            }

            if (!string.IsNullOrEmpty(searchCriteria.SortField))
            {
                var sortDirection = searchCriteria.Ascending ? "ascending" : "descending";
                query = query.OrderBy($"{searchCriteria.SortField} {sortDirection}");
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
