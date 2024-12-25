using Dapper;
using NokCore.Exceptions;
using NokPortalAPI.Domains.Models;
using System.Data;

namespace NokPortalAPI.Domains.Repositorys
{
    /// <summary>
    /// Repository for managing app environments data.
    /// </summary>
    public class AppEnvRepository : IAppEnvRepository
    {
        /// <summary>
        /// Create a new app environment record.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="appEnv">App environment model.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task CreateAsync(IDbConnection conn, AppEnv appEnv, IDbTransaction? tran = null)
        {
            const string checkSql = @"
                SELECT COUNT(*)
                FROM Apps_Env
                WHERE AppID = @AppID AND Environment = @Environment;";

            const string insertSql = @"
                INSERT INTO Apps_Env (AppID, Environment, BaseURL, Additional, SecretKey, JwtHourLimit)
                VALUES (@AppID, @Environment, @BaseURL, @Additional, @SecretKey, @JwtHourLimit);";

            // Check if a record already exists
            int count = await conn.ExecuteScalarAsync<int>(checkSql, appEnv, tran);

            if (count > 0)
            {
                throw new DataValidationException("Duplicate record found.");
            }

            // Insert the record
            await conn.ExecuteAsync(insertSql, appEnv, tran);
        }

        /// <summary>
        /// Update an existing app environment record.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="appEnv">App environment model.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task UpdateAsync(IDbConnection conn, AppEnv appEnv, IDbTransaction? tran = null)
        {
            const string checkSql = @"
                SELECT COUNT(*)
                FROM Apps_Env
                WHERE AppID = @AppID AND Environment = @Environment;";

            const string updateSql = @"
                UPDATE Apps_Env
                SET BaseURL = @BaseURL, Additional = @Additional, SecretKey = @SecretKey, JwtHourLimit = @JwtHourLimit
                WHERE AppID = @AppID AND Environment = @Environment;";

            // Check if a record already exists
            int count = await conn.ExecuteScalarAsync<int>(checkSql, appEnv, transaction: tran);

            if (count == 0)
            {
                throw new DataNotFoundException("Record not found.");
            }

            // Update the record
            await conn.ExecuteAsync(updateSql, appEnv, transaction: tran);
        }

        /// <summary>
        /// Get an app environment record by ID for an admin user.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="appID">App ID.</param>
        /// <param name="userID">User ID.</param>
        /// <param name="env">Environment type.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="DataValidationException"></exception>
        public async Task<AppEnv> GetByIdForAdminAsync(IDbConnection conn, int appID, int userID, EnumEnvironmentType env, IDbTransaction? tran = null)
        {
            // Check if the userId exists in Assigned_Users table.
            var userExists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Assigned_Users WHERE UserId = @UserId AND AppId = @AppId;",
                new { UserID = userID, AppID = appID },
                transaction: tran);

            if (userExists == 0)
            {
                throw new DataValidationException("This user does not have access, or user/application cannot be found in system.");
            }

            const string sql = "SELECT * FROM Apps_Env WHERE AppID = @AppID AND Environment = @Env";
            AppEnv? appEnv = await conn.QuerySingleOrDefaultAsync<AppEnv>(sql, new { AppID = appID, Env = (int)env }, tran);

            if (appEnv == null)
            {
                throw new DataValidationException("No app environment found.");
            }

            return appEnv;
        }

        /// <summary>
        /// Get an app environment record by ID.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="appID">App ID.</param>
        /// <param name="env">Environment type.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="DataValidationException"></exception>
        public async Task<AppEnv> GetByIdAsync(IDbConnection conn, int appID, EnumEnvironmentType env, IDbTransaction? tran = null)
        {
            const string sql = "SELECT * FROM Apps_Env WHERE AppID = @AppID AND Environment = @Env";
            AppEnv? appEnv = await conn.QuerySingleOrDefaultAsync<AppEnv>(sql, new { AppID = appID, Env = (int)env }, tran);

            if (appEnv == null)
            {
                throw new DataValidationException("No app environment found.");
            }

            return appEnv;
        }

        /// <summary>
        /// Get all app environments.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        public async Task<IEnumerable<AppEnv>> GetAllAsync(IDbConnection conn, IDbTransaction? tran = null)
        {
            const string sql = "SELECT * FROM Apps_Env";
            return await conn.QueryAsync<AppEnv>(sql, transaction: tran);
        }

        /// <summary>
        /// Delete an app environment record by app ID.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <param name="appID">App ID.</param>
        /// <returns></returns>
        public async Task DeleteByAppIdAsync(IDbConnection conn, int appID, IDbTransaction? tran = null)
        {
            const string sql = "DELETE FROM Apps_Env WHERE AppID = @AppID";
            await conn.ExecuteAsync(sql, new { AppID = appID }, tran);
        }
    }
}
