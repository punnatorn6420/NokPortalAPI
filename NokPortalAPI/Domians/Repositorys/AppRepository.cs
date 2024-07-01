using System.Data;
using System.Data.SqlClient;
using Dapper;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;

namespace NokPortal.Domains.Repositories
{
    public class AppRepository : IAppRepository
    {
        public async Task<bool> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestCreateApp reqCreate)
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

        public async Task<IEnumerable<ModelApp>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran)
        {
            var sql = "SELECT * FROM Apps";
            return await conn.QueryAsync<ModelApp>(sql, transaction: tran);
        }

        public async Task<ModelApp> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            var sql = "SELECT * FROM Apps WHERE AppId = @AppId";
            ModelApp? app = await conn.QueryFirstOrDefaultAsync<ModelApp>(sql, new { AppID = id }, transaction: tran);
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
