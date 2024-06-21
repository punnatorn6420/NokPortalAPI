using System.Data;
using Dapper;
using NokPortal.Domians.Models;

namespace NokPortal.Domians.Repositorys
{
    public class AppEnvRepository : IAppEnvRepository
    {
        public async Task Create(IDbConnection conn, IDbTransaction tran, RequestCreateAppEnv model) // CreateAppsEnvAsync
        {
            const string sql = "INSERT INTO Apps_Env (AppId, Environment, BaseURL, Additional, SecretKey, JwtHourLimit) " +
                   "VALUES (@AppId, @Environment, @BaseURL, @Additional, @SecretKey, @JwtHourLimit); " +
                   "SELECT @@ROWCOUNT;";
            int rowCount = await conn.ExecuteScalarAsync<int>(sql, model, tran);

            if (rowCount == 0)
            {
                throw new Exception("Insert operation failed.");
            }
        }

        public async Task<RequestCreateAppEnv> GetById(IDbConnection conn, IDbTransaction tran, int appId, int userId)
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
            RequestCreateAppEnv? appsEnvModel = await conn.QueryFirstOrDefaultAsync<RequestCreateAppEnv>(sql, new { AppID = appId }, tran);
            if (appsEnvModel == null)
            {
                throw new Exception("Not found apps environment");
            }
            return appsEnvModel;
        }

        public async Task<IEnumerable<RequestCreateAppEnv>> GetAll(IDbConnection conn, IDbTransaction tran)
        {
            const string sql = "SELECT * FROM Apps_Env";
            return await conn.QueryAsync<RequestCreateAppEnv>(sql, transaction: tran);
        }

        public async Task Update(IDbConnection conn, IDbTransaction tran, RequestCreateAppEnv model)
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
