using Microsoft.Data.SqlClient;
using NokAir.Shared.Infrastructures;
using System.Data;

namespace NokPortalAPI.Shareds.DB
{
    /// <summary>
    /// Database connection factory.
    /// </summary>
    public class DbConnectionFactory : DbConnectionFactoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbConnectionFactory"/> class.
        /// </summary>
        /// <param name="configuration"></param>
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
            return new SqlConnection(Configuration["ConnectionStrings:NokPortalDB"]);
        }

        /// <summary>
        /// Implements the CreateConnectionAsync method from the BaseDbConnectionFactory class.
        /// </summary>
        public override async Task<IDbConnection> CreateConnectionAsync()
        {
            var conn = new SqlConnection(Configuration["ConnectionStrings:NokPortalDB"]);
            await conn.OpenAsync();
            return conn;
        }
    }
}