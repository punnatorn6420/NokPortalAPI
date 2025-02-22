using System.Data;
using Dapper;
using NokCore.Exceptions;
using NokPortalAPI.Models;

namespace NokPortalAPI.Repositories.Old
{
    /// <summary>
    /// Repository for managing app data.
    /// </summary>
    public class AppRepository : IAppRepository
    {
        /// <summary>
        /// Create a new app record.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="app">Request to create a new app.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="DataValidationException"></exception>
        public async Task<bool> CreateAppAsync(IDbConnection conn, App app, IDbTransaction? tran = null)
        {
            try
            {
                // Check if the name already exists
                var checkSql = "SELECT COUNT(1) FROM Apps WHERE Name = @Name";
                var nameExists = await conn.ExecuteScalarAsync<bool>(checkSql, new { app.Name }, transaction: tran);

                if (nameExists)
                {
                    throw new DataValidationException("An app with this name already exists.");
                }

                // Insert new record
                var insertSql = @"
                    INSERT INTO Apps (Name, Header, Subheader, Detail, Image ,BaseUrl)
                    VALUES (@Name, @Header, @Subheader, @Detail, @Image , @BaseUrl);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                await conn.ExecuteScalarAsync<int>(insertSql, app, transaction: tran);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Update an existing app record.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="app">Request to update an app.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        /// <exception cref="DataNotFoundException"></exception>
        public async Task<bool> UpdateAppAsync(IDbConnection conn, App app, IDbTransaction? tran = null)
        {
            var checkSql = "SELECT COUNT(1) FROM Apps WHERE Name = @Name";
            var appExists = await conn.ExecuteScalarAsync<int>(checkSql, new { app.Name }, transaction: tran) > 0;

            if (!appExists)
            {
                throw new DataNotFoundException("App not found.");
            }

            var updateSql = @"
                    UPDATE Apps
                    SET Header = @Header, Subheader = @Subheader, Detail = @Detail, Image = @Image, BaseUrl = @BaseUrl
                    WHERE Name = @Name;";

            await conn.ExecuteAsync(updateSql, app, transaction: tran);
            return true;
        }

        /// <summary>
        /// Get all app records.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <returns></returns>
        public async Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction? tran = null)
        {
            var sql = "SELECT * FROM Apps";
            return await conn.QueryAsync<App>(sql, transaction: tran);
        }

        /// <summary>
        /// Get an app record by ID.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <param name="id">App ID.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<App> GetAppByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null)
        {
            var sql = "SELECT * FROM Apps WHERE AppId = @AppId";
            App? app = await conn.QueryFirstOrDefaultAsync<App>(sql, new { AppID = id }, transaction: tran);
            if (app == null)
            {
                throw new DataNotFoundException("Not found app");
            }
            return app;
        }

        /// <summary>
        /// Delete an app record by ID.
        /// </summary>
        /// <param name="conn">Connection to the database.</param>
        /// <param name="tran">Transaction to use.</param>
        /// <param name="id">App ID.</param>
        /// <returns></returns>
        public async Task<bool> DeleteAppByIdAsync(IDbConnection conn, int id, IDbTransaction? tran = null)
        {
            var sql = "DELETE FROM Apps WHERE AppId = @AppId";
            var rowsAffected = await conn.ExecuteAsync(sql, new { AppID = id }, transaction: tran);
            return rowsAffected > 0;
        }
    }
}
