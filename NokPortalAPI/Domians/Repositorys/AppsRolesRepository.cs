using Dapper;
using NokPortalAPI.Domains.Models;
using System.Data;

namespace NokPortalAPI.Domains.Repositorys
{
    public class AppsRolesRepository : IAppsRolesRepositorys
    {
        public async Task<IEnumerable<AppsRole>> GetAllAsync()
        {
            // Implement method
            throw new NotImplementedException();
        }

        public async Task<AppsRole> GetByAssignedUserIdAsync(int assignedUserId)
        {
            // Implement method
            throw new NotImplementedException();
        }

        public async Task<int> AddAsync(AppsRole appsRole)
        {
            // Implement method
            throw new NotImplementedException();
        }

        public async Task<int> DeleteAsync(int assignedUserId, int roleId)
        {
            // Implement method
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<AppWithRoles>> GetAppsWithRolesByUserIdAsync(IDbConnection conn, IDbTransaction tran, int userId, EnumEnvironmentType env)
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
