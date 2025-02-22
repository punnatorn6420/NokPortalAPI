using System.Data;
using Dapper;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    /// <summary>
    /// Repository for managing apps roles data.
    /// </summary>
    public class AppsRolesRepository : IAppsRolesRepositorys
    {
        /// <summary>
        /// Get all apps roles.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<AppsRole>> GetAllAsync()
        {
            // Implement method
            throw new NotImplementedException();
        }

        public Task<AppsRole> GetByAssignedUserIdAsync(int assignedUserId)
        {
            // Implement method
            throw new NotImplementedException();
        }

        public Task<int> AddAsync(AppsRole appsRole)
        {
            // Implement method
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int assignedUserId, int roleId)
        {
            // Implement method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all apps roles by user id.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <param name="userId">User id.</param>
        /// <param name="env">Environment type.</param>
        /// <returns></returns>
        public async Task<IEnumerable<AppWithRoles>> GetAppsWithRolesByUserIdAsync(IDbConnection conn, int userId, EnumEnvironmentType env, IDbTransaction? tran = null)
        {
            string appQuery = @"
                SELECT 
                    a.AppId, a.Name, a.Header, a.Subheader, a.Detail, a.BaseUrl , a.Image, 
                    ar.RoleID,
                    ua.Environment
                FROM Assigned_Users ua
                INNER JOIN Apps a ON ua.AppId = a.AppId
                LEFT JOIN Apps_Roles ar ON ua.AssignedUserID = ar.AssignedUserID
                WHERE ua.UserID = @UserId";
            var appDictionary = new Dictionary<int, AppWithRoles>();

            var result = await conn.QueryAsync<AppWithRoles, int?, AppWithRoles>(
                appQuery,
                (app, roleId) =>
                {
                    if (!appDictionary.TryGetValue(app.AppId, out var appWithRoles))
                    {
                        appWithRoles = app;
                        appWithRoles.Roles = new List<int>();
                        appDictionary.Add(app.AppId, appWithRoles);
                    }

                    if (roleId.HasValue)
                    {
                        appWithRoles.Roles.Add(roleId.Value);
                    }

                    return appWithRoles;
                },
                new { UserId = userId, Environment = (int)env },
                splitOn: "RoleID",
                transaction: tran);

            return appDictionary.Values;
        }
    }
}
