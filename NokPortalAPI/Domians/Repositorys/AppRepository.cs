using System.Data;
using System.Data.SqlClient;
using Dapper;
using NokPortalAPI.Domains.Models;
using NokPortalAPI.Domains.Repositorys;

namespace NokPortalAPI.Domains.Repositorys
{
    public class AppRepository : IAppRepository
    {
        public async Task<bool> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqCreate)
        {
            try
            {
                // Check if the name already exists
                var checkSql = "SELECT COUNT(1) FROM Apps WHERE Name = @Name";
                var nameExists = await conn.ExecuteScalarAsync<bool>(checkSql, new { reqCreate.Name }, transaction: tran);

                if (nameExists)
                {
                    throw new DuplicateNameException("An app with this name already exists.");
                }

                // Insert new record
                var insertSql = @"
                    INSERT INTO Apps (Name, Header, Subheader, Detail, Image)
                    VALUES (@Name, @Header, @Subheader, @Detail, @Image);
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                await conn.ExecuteScalarAsync<int>(insertSql, reqCreate, transaction: tran);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, RequestApp reqUpdate)
        {
            var checkSql = "SELECT COUNT(1) FROM Apps WHERE Name = @Name";
            var appExists = await conn.ExecuteScalarAsync<int>(checkSql, new { reqUpdate.Name }, transaction: tran) > 0;

            if (!appExists)
            {
                throw new KeyNotFoundException("App not found.");
            }

            var updateSql = @"
                    UPDATE Apps
                    SET Header = @Header, Subheader = @Subheader, Detail = @Detail, Image = @Image
                    WHERE Name = @Name;";

            await conn.ExecuteAsync(updateSql, reqUpdate, transaction: tran);

            return true;
        }

        public async Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran)
        {
            var sql = "SELECT * FROM Apps";
            return await conn.QueryAsync<App>(sql, transaction: tran);
        }

        public async Task<App> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            var sql = "SELECT * FROM Apps WHERE AppId = @AppId";
            App? app = await conn.QueryFirstOrDefaultAsync<App>(sql, new { AppID = id }, transaction: tran);
            if (app == null)
            {
                throw new Exception("Not found app");
            }
            return app;
        }

        public async Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, int id, string name)
        {
            var sql = "UPDATE Apps SET Name = @Name WHERE AppId = @AppId";
            var rowsAffected = await conn.ExecuteAsync(sql, new { AppID = id, Name = name }, transaction: tran);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAppAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            var sql = "DELETE FROM Apps WHERE AppId = @AppId";
            var rowsAffected = await conn.ExecuteAsync(sql, new { AppID = id }, transaction: tran);
            return rowsAffected > 0;
        }
    }
}
