using System.Data;
using Dapper;
using NokCore.Exceptions;
using NokPortalAPI.Domains.Models;

namespace NokPortalAPI.Domains.Repositorys
{
    /// <summary>
    /// Repository for managing users that are assigned to applications data.
    /// </summary>
    public class AssignedUsersRepositorys : IAssignedUsersRepositorys
    {
        /// <summary>
        /// Get assigned user by application id and user id.
        /// </summary>
        /// <param name="conn">Database connection.</param>
        /// <param name="tran">Database transaction.</param>
        /// <param name="appId">Application id.</param>
        /// <param name="userId">User id.</param>
        /// <returns>Assigned user.</returns>
        /// <exception cref="Exception"></exception>
        public async Task<ModelUserApp> GetAssignedUserAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            var sql = "SELECT * FROM Assigned_Users WHERE AppId = @AppId AND UserId = @UserId;";
            ModelUserApp? userApp = await conn.QueryFirstOrDefaultAsync<ModelUserApp>(sql, new { AppId = appId, UserId = userId }, transaction: tran);
            if (userApp == null)
            {
                throw new DataNotFoundException("Not found assigned user.");
            }

            return userApp;
        }

        /// <summary>
        /// Get all assigned users for an application.
        /// </summary>
        /// <param name="conn">Database connection.</param>
        /// <param name="tran">Database transaction.</param>
        /// <param name="appId">Application id.</param>
        /// <returns>Assigned users.</returns>
        public async Task<IEnumerable<App>> GetAllAssingedUsersAppAsync(IDbConnection conn, IDbTransaction tran, int userId)
        {
            const string sql = @"
                SELECT a.*
                FROM Apps a
                INNER JOIN Assigned_Users ua ON a.AppId = ua.AppId
                WHERE ua.UserId = @UserId";

            var userApps = await conn.QueryAsync<App>(sql, new { UserId = userId }, tran);

            if (!userApps.Any())
            {
                return [];
            }

            return userApps;
        }

        /// <summary>
        /// Create assigned users.
        /// </summary>
        /// <param name="conn">Database connection.</param>
        /// <param name="tran">Database transaction.</param>
        /// <param name="appId">Application id.</param>
        /// <param name="userId">User id.</param>
        /// <param name="roles">Roles of the target application.</param>
        /// <param name="environment">Application environment.</param>
        /// <returns></returns>
        public async Task CreateAssignedUsersAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, IEnumerable<int> roles, EnumEnvironmentType environment)
        {
            // Ensure the connection is open
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            // Insert into Assigned_Users table
            var insertAssignedUserQuery = @"
                    INSERT INTO Assigned_Users (UserID, AppID, Environment)
                    VALUES (@UserID, @AppID, @Environment);
                    SELECT SCOPE_IDENTITY();
                ";

            // Insert into Apps_Roles table
            var insertAppRolesQuery = @"
                    INSERT INTO Apps_Roles (AssignedUserID, RoleID)
                    VALUES (@AssignedUserID, @RoleID);
                ";

            try
            {
                // Insert Assigned User
                var parameters = new { UserID = userId, AppID = appId, Environment = environment.ToString() };
                int assignedUserId = await conn.ExecuteScalarAsync<int>(insertAssignedUserQuery, parameters, tran);

                // Log AssignedUserID

                // Insert App Roles
                foreach (var role in roles)
                {
                    var roleParams = new { AssignedUserID = assignedUserId, RoleID = role };
                    int rowsAffected = await conn.ExecuteAsync(insertAppRolesQuery, roleParams, tran);

                    // Log the result of each insert
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error in AssignedUsersAsync: {ex.Message}");
                throw;
            }
        }
    }
}
