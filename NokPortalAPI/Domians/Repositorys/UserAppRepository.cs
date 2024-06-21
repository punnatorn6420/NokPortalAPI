using System.Data;
using Dapper;
using NokCore.Identity.Models;
using NokPortal.Domains.Models;

namespace NokPortal.Domains.Repositories
{
    public class UserAppRepository : IUserAppRepository
    {
        public async Task<int> AddUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId, int roleId)
        {
            // Check if AppID exists
            var appExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Apps WHERE AppId = @AppId;",
                new { AppID = appId },
                transaction: tran);

            // Check if UserID exists
            var userExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE UserId = @UserId;",
                new { UserID = userId },
                transaction: tran);

            // If either AppID or UserID does not exist, return 0 (indicating failure)
            if (appExists == 0 || userExists == 0)
            {
                throw new Exception("Not found AppId or UserId");
            }

            var sql = "INSERT INTO Users_Apps (AppId, UserId, RoleId) VALUES (@AppId, @UserId, @RoleId);";
            return await conn.ExecuteAsync(sql, new { AppId = appId, UserId = userId, RoleId = roleId }, transaction: tran);

        }

        public async Task<int> DeleteUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            var sql = "DELETE FROM Users_Apps WHERE AppId = @AppId AND UserId = @UserId;";
            return await conn.ExecuteAsync(sql, new { AppID = appId, UserID = userId }, transaction: tran);
        }

        public async Task<UserApp> GetUserAppAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            var sql = "SELECT * FROM Users_Apps WHERE AppId = @AppId AND UserId = @UserId;";
            UserApp? userApp = await conn.QueryFirstOrDefaultAsync<UserApp>(sql, new { AppId = appId, UserId = userId }, transaction: tran);
            if (userApp == null)
            {
                throw new Exception("Not found userApp");
            }
            return userApp;
        }

        public async Task<IEnumerable<UserApp>> GetAllUserAppsAsync(IDbConnection conn, IDbTransaction tran)
        {
            var sql = "SELECT * FROM Users_Apps;";
            return await conn.QueryAsync<UserApp>(sql);
        }
    }
}
