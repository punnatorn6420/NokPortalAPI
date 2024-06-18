using System.Data;
using Dapper;
using NokPortal.Domains.Models;

namespace NokPortal.Domains.Repositories
{
    public class UsersAppsRepository : IUsersAppsRepository
    {
        public async Task<int> AddUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            // Check if AppID exists
            var appExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Apps WHERE AppID = @AppID;",
                new { AppID = appId },
                transaction: tran);

            // Check if UserID exists
            var userExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE UserID = @UserID;",
                new { UserID = userId },
                transaction: tran);

            // If either AppID or UserID does not exist, return 0 (indicating failure)
            if (appExists == 0 || userExists == 0)
            {
                throw new Exception("Not found AppID or UserID");
            }

            var sql = "INSERT INTO Users_Apps (AppID, UserID) VALUES (@AppID, @UserID);";
            return await conn.ExecuteAsync(sql, new { AppID = appId, UserID = userId }, transaction: tran);
        }

        public async Task<int> DeleteUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            var sql = "DELETE FROM Users_Apps WHERE AppID = @AppID AND UserID = @UserID;";
            return await conn.ExecuteAsync(sql, new { AppID = appId, UserID = userId }, transaction: tran);
        }

        public async Task<UserApp> GetUserAppAsync(IDbConnection conn, int appId, int userId)
        {
            var sql = "SELECT * FROM Users_Apps WHERE AppID = @AppID AND UserID = @UserID;";
            UserApp? userApp = await conn.QuerySingleOrDefaultAsync<UserApp>(sql, new { AppID = appId, UserID = userId });
            if (userApp == null)
            {
                throw new Exception("Not found userApp");
            }
            return userApp;
        }

        public async Task<IEnumerable<UserApp>> GetAllUserAppsAsync(IDbConnection conn)
        {
            var sql = "SELECT * FROM Users_Apps;";
            return await conn.QueryAsync<UserApp>(sql);
        }
    }
}
