using System.Data;
using System.Data.SqlClient;
using Dapper;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    public class AssignedUsersRepositorys : IAssignedUsersRepositorys
    {
        public async Task<ModelUserApp> GetUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            var sql = "SELECT * FROM Assigned_Users WHERE AppId = @AppId AND UserId = @UserId;";
            ModelUserApp? userApp = await conn.QueryFirstOrDefaultAsync<ModelUserApp>(sql, new { AppId = appId, UserId = userId }, transaction: tran);
            if (userApp == null)
            {
                throw new Exception("Not found userApp");
            }
            return userApp;
        }

        public async Task<IEnumerable<App>> GetUserAllAppAsync(IDbConnection conn, IDbTransaction tran, int userId)
        {
            const string sql = @"
                SELECT a.*
                FROM Apps a
                INNER JOIN Assigned_Users ua ON a.AppId = ua.AppId
                WHERE ua.UserId = @UserId";

            var userApps = await conn.QueryAsync<App>(sql, new { UserId = userId }, tran);

            if (!userApps.Any())
            {
                throw new Exception("No applications found for this user.");
            }

            return userApps;
        }

        public async Task AssignedUsersAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, IEnumerable<int> roles)
        {
            // Ensure the connection is open
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            // Insert into Assigned_Users table
            var insertAssignedUserQuery = @"
                INSERT INTO Assigned_Users (UserID, AppID)
                VALUES (@UserID, @AppID);
            ";

            // Insert into Apps_Roles table
            var insertAppRolesQuery = @"
                INSERT INTO Apps_Roles (AssignedUserID, RoleID)
                VALUES (@AssignedUserID, @RoleID);
            ";

            // Insert Assigned User
            var parameters = new { UserID = userId, AppID = appId };
            int assignedUserId = await conn.ExecuteScalarAsync<int>(insertAssignedUserQuery, parameters, tran);

            // Insert App Roles
            foreach (var role in roles)
            {
                var roleParams = new { AssignedUserID = assignedUserId, RoleID = role };
                await conn.ExecuteAsync(insertAppRolesQuery, roleParams, tran);
            }
        }
    }
}
