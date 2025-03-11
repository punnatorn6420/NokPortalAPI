using Microsoft.EntityFrameworkCore;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    public class RoleRepository : IRoleRepository<Role>
    {
        private readonly AppDbContext context;

        public RoleRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc/>
        public async Task<Role> AddRoleAsync(Role role)
        {
            var result = await context.Roles.AddAsync(role);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc/>
        public async Task<int> DeleteRoleByIdAsync(int roleId)
        {
            var role = await context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return 0;
            }

            context.Roles.Remove(role);
            return await context.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await context.Roles.FindAsync(roleId);
        }

        /// <inheritdoc/>
        public async Task<ICollection<Role>> GetRolesByCriteriaAsync(RoleSearchCriteria searchCriteria)
        {
            IQueryable<Role> query = context.Roles;
            if (!string.IsNullOrEmpty(searchCriteria.Keyword))
            {
                query = query.Where(r => r.Name.Contains(searchCriteria.Keyword));
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
        public async Task<bool> IsUserInRoleAsync(int userId, string requiredRole)
        {
            return await context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == requiredRole);
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserInRolesAsync(int userId, string[] requiredRoles)
        {
            return await context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && requiredRoles.Contains(ur.Role.Name));
        }

        /// <inheritdoc/>
        public async Task<int> UpdateRoleAsync(Role role)
        {
            context.Roles.Update(role);
            return await context.SaveChangesAsync();
        }
    }
}
