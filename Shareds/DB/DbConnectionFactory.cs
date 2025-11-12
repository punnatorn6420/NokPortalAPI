namespace NokPortalAPI.Shareds.DB
{
    using Microsoft.Data.SqlClient;
    using NokAir.Shared.Infrastructures;
    using System.Data;

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