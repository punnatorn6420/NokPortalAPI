using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NokAir.Core.Interfaces.Rbac.Entities;
using NokPortalAPI.Entities;
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
        public async Task<ICollection<Role>> GetRolesByCriteriaAsync(IRoleSearchCriteria searchCriteria)
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
                .AnyAsync(ur => ur.UserId == userId && ur.Role!.Name == requiredRole);
        }

        /// <inheritdoc/>
        public async Task<bool> IsUserInRolesAsync(int userId, string[] requiredRoles)
        {
            var rolesList = string.Join(",", requiredRoles.Select(r => $"'{r}'"));
            var sqlQuery = $@"
                SELECT COUNT(*) AS RoleCount
                FROM User_Role ur
                INNER JOIN Roles r ON ur.RoleId = r.Id
                WHERE ur.UserId = @userId AND r.Name IN ({rolesList})";

            var count = await context.UserRoles
                .FromSqlRaw(sqlQuery, new SqlParameter("@userId", userId))
                .CountAsync();

            return count > 0;
        }

        /// <inheritdoc/>
        public async Task<int> UpdateRoleAsync(Role role)
        {
            context.Roles.Update(role);
            return await context.SaveChangesAsync();
        }

        public async Task AssignDefaultRoleAsync(int userId, int defaultRoleId = 3)
        {

            if (defaultRoleId <= 0)
                throw new ArgumentException("Invalid default role ID");

            var exists = await context.Roles.AnyAsync(r => r.Id == defaultRoleId);
            if (!exists)
                throw new Exception($"Role ID {defaultRoleId} does not exist");

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = defaultRoleId,
                Role = null,
                User = null
            };
            await context.UserRoles.AddAsync(userRole);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<IRole>> GetAllRolesAsync()
        {
            return await context.Roles
                .AsNoTracking()
                .Cast<IRole>()
                .ToListAsync();
        }

    }
}
