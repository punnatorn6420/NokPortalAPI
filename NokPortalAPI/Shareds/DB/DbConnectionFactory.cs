using System.Data;
using System.Data.SqlClient;

namespace NokPortalAPI.Shared.DB
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }

    public class DbConnectionFactory : IDbConnectionFactory
    {
        private readonly IConfiguration _configuration;

        public DbConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_configuration["ConnectionStrings:NokPortalDB"]);
        }
    }
}