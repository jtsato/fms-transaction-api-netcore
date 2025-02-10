using System.Data.Common;
using Npgsql;

namespace Infra.PostgreSql.Commons.Connection;

public sealed class ConnectionFactory(string connectionString) : IConnectionFactory
{

    public DbConnection GetConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
