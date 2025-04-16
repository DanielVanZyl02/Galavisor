using System.Data;
using Npgsql;

namespace GalavisorApi.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual IDbConnection CreateConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}