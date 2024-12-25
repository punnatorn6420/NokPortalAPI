using System.Data;
using System.Data.SqlClient;
using NokCore.Db;

namespace NokPortalAPI.Shared.DB
{
    public class DbConnectionFactory : BaseDbConnectionFactory
    {
        public DbConnectionFactory(IConfiguration configuration)
            : base(configuration)
        {
        }

        /// <summary>
        /// Implements the CreateConnection method from the BaseDbConnectionFactory class.
        /// </summary>
        /// <returns></returns>
        public override IDbConnection CreateConnection()
        {
            return new SqlConnection(this.configuration["ConnectionStrings:NokPortalDB"]);
        }

        /// <summary>
        /// Implements the CreateConnectionAsync method from the BaseDbConnectionFactory class.
        /// </summary>
        public override async Task<IDbConnection> CreateConnectionAsync()
        {
            var conn = new SqlConnection(this.configuration["ConnectionStrings:NokPortalDB"]);
            await conn.OpenAsync();
            return conn;
        }
    }
}