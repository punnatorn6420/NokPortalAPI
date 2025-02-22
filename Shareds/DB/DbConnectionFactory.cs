namespace NokPortalAPI.Shareds.DB
{
    using System.Data;
    using Microsoft.Data.SqlClient;
    using NokCore.Db;

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
            return new SqlConnection(configuration["ConnectionStrings:NokPortalDB"]);
        }

        /// <summary>
        /// Implements the CreateConnectionAsync method from the BaseDbConnectionFactory class.
        /// </summary>
        public override async Task<IDbConnection> CreateConnectionAsync()
        {
            var conn = new SqlConnection(configuration["ConnectionStrings:NokPortalDB"]);
            await conn.OpenAsync();
            return conn;
        }
    }
}