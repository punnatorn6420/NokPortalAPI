using System.Data;
using Dapper;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public class AppEnvRepository : IAppEnvRepository
    {
        public async Task Create(IDbConnection conn, IDbTransaction tran, AppEnv model)
        {
            const string checkSql = @"
                SELECT COUNT(*)
                FROM Apps_Env
                WHERE AppId = @AppId AND Environment = @Environment;";

            const string insertSql = @"
                INSERT INTO Apps_Env (AppId, Environment, BaseURL, Additional, SecretKey, JwtHourLimit)
                VALUES (@AppId, @Environment, @BaseURL, @Additional, @SecretKey, @JwtHourLimit);";

            // Check if a record already exists
            int count = await conn.ExecuteScalarAsync<int>(checkSql, model, tran);

            if (count > 0)
            {
                throw new Exception("Duplicate record found.");
            }

            // Insert the record
            await conn.ExecuteAsync(insertSql, model, tran);
        }

        public async Task<IEnumerable<AppEnv>> GetByIdAsync(IDbConnection conn, IDbTransaction tran, int appId, int userId)
        {
            // Check if the userId exists in Users_Apps
            var userExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users_Apps WHERE UserId = @UserId AND AppId = @AppId;",
                new { UserID = userId, AppID = appId },
                transaction: tran);

            if (userExists == 0)
            {
                throw new UnauthorizedAccessException("This user does not have access, or user/application cannot be found in system.");
            }

            const string sql = "SELECT * FROM Apps_Env WHERE AppId = @AppId";
            IEnumerable<AppEnv> appsEnvModels = await conn.QueryAsync<AppEnv>(sql, new { AppID = appId }, tran);

            if (!appsEnvModels.Any())
            {
                throw new Exception("No apps environment found.");
            }

            return appsEnvModels;
        }

        public async Task<IEnumerable<AppEnv>> GetAll(IDbConnection conn, IDbTransaction tran)
        {
            const string sql = "SELECT * FROM Apps_Env";
            return await conn.QueryAsync<AppEnv>(sql, transaction: tran);
        }

        public async Task Update(IDbConnection conn, IDbTransaction tran, AppEnv model)
        {
            const string sql = "UPDATE Apps_Env SET Environment = @Environment, BaseURL = @BaseURL, " +
                               "Additional = @Additional, SecretKey = @SecretKey, JwtHourLimit = @JwtHourLimit " +
                               "WHERE AppId = @AppId";
            await conn.ExecuteAsync(sql, model, tran);
        }

        public async Task Delete(IDbConnection conn, IDbTransaction tran, int appId)
        {
            const string sql = "DELETE FROM Apps_Env WHERE AppId = @AppId";
            await conn.ExecuteAsync(sql, new { AppID = appId }, tran);
        }
    }
}
