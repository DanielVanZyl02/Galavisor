using System.Data;
using Npgsql;

namespace GalavisorApi.Data;

public class TestDatabaseConnection : DatabaseConnection
{
    private readonly IDbConnection _connection;

    public TestDatabaseConnection(IDbConnection connection) : base("test_connection_string")
    {
        _connection = connection;
    }

    public override IDbConnection CreateConnection()
    {
        return _connection;
    }
}