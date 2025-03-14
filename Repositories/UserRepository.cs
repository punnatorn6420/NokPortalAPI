using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NokCore.Identity.Models;
using NokPortalAPI.Models;
using System.Linq.Dynamic.Core;

namespace NokPortalAPI.Repositories
{
    /// <summary>
    /// User repository.
    /// </summary>
    public class UserRepository : IUserRepository<User>
    {
        private readonly AppDbContext context;

        public UserRepository(AppDbContext context)
        {
            this.context = context;
        }

        /// <inheritdoc />
        public async Task<User> AddUserAsync(User user)
        {
            var result = await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return result.Entity;
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await context.Users
               .Include(u => u.UserRoles)
                   .ThenInclude(ur => ur.Role)
               .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByAppIdAsync(int appId)
        {
            // Get users by application ID from AssignedUserApps table.
            var query = from aua in context.UserAppRoleAssignments
                        join u in context.Users on aua.UserId equals u.Id
                        where aua.AppId == appId
                        select u;

            return await query.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<ICollection<User>> GetUsersByCriteriaAsync(UserSearchCriteria searchCriteria)
        {
            var sortField = !string.IsNullOrEmpty(searchCriteria.SortField) ? searchCriteria.SortField : "Id";
            var sortDirection = searchCriteria.Ascending ? "ASC" : "DESC";
            var sqlQuery = $@"
                SELECT * FROM (
                    SELECT *, ROW_NUMBER() OVER (ORDER BY {sortField} {sortDirection}) AS RowNum
                    FROM Users
                    WHERE
                        FirstName LIKE @keyword
                        OR LastName LIKE @keyword
                        OR Email LIKE @keyword
                ) AS Result
                WHERE RowNum BETWEEN @startRow AND @endRow";

            var keywordParam = new SqlParameter("@keyword", $"%{searchCriteria.Keyword}%");
            var startRowParam = new SqlParameter("@startRow", (searchCriteria.PageNumber - 1) * searchCriteria.PageSize + 1);
            var endRowParam = new SqlParameter("@endRow", searchCriteria.PageNumber * searchCriteria.PageSize);

            return await context.Users.FromSqlRaw(sqlQuery, keywordParam, startRowParam, endRowParam).ToListAsync();
        }



        /// <inheritdoc />
        public async Task<int> UpdateUserAsync(User user)
        {
            context.Users.Update(user);
            return await context.SaveChangesAsync();
        }
    }
}
