using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Entities
{
    public class BgfclContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public BgfclContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("AppContext");
        }
        public IDbConnection CreateConnection()
       => new SqlConnection(_connectionString);

    }
}
