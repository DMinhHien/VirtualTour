using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace VirtualTour.DataAccess
{
    public interface IDbContext
    {
        IDbConnection CreateConnection();
    }
    public class SqlDbContext : IDbContext
    {
        private readonly string _connectionString;
        
        public SqlDbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
