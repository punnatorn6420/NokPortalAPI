using System.Data;
using Dapper;
using NokPortal.Domians.Models;
using NokPortal.Domians.Repositorys;

namespace NokPortal.Domains.Repositories
{
    public class AppRepository : IAppRepository
    {
        public async Task<int> CreateAppAsync(IDbConnection conn, IDbTransaction tran, RequestCreateApp reqCreate)
        {
            var sql = @"
                INSERT INTO Apps (Name, Header, Subheader, Detail, Image)
                VALUES (@Name, @Header, @Subheader, @Detail, @Image);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            return await conn.ExecuteScalarAsync<int>(sql, reqCreate, transaction: tran);
        }

        public async Task<IEnumerable<App>> GetAllAppAsync(IDbConnection conn, IDbTransaction tran)
        {
            var sql = "SELECT * FROM Apps";
            return await conn.QueryAsync<App>(sql, transaction: tran);
        }

        public async Task<App> GetAppByIdAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            var sql = "SELECT * FROM Apps WHERE AppID = @AppID";
            App? app = await conn.QueryFirstOrDefaultAsync<App>(sql, new { AppID = id }, transaction: tran);
            if (app == null)
            {
                throw new Exception("Not found app");
            }
            return app;
        }

        public async Task<bool> UpdateAppAsync(IDbConnection conn, IDbTransaction tran, int id, string name)
        {
            var sql = "UPDATE Apps SET Name = @Name WHERE AppID = @AppID";
            var rowsAffected = await conn.ExecuteAsync(sql, new { AppID = id, Name = name }, transaction: tran);
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAppAsync(IDbConnection conn, IDbTransaction tran, int id)
        {
            var sql = "DELETE FROM Apps WHERE AppID = @AppID";
            var rowsAffected = await conn.ExecuteAsync(sql, new { AppID = id }, transaction: tran);
            return rowsAffected > 0;
        }
    }
}
