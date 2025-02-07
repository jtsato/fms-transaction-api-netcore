using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Infra.PostgreSQL.Commons.Connection;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest.Infra.PostgreSql.Commons;

public sealed class DatabaseKeeper
{
    private readonly IConnectionFactory _connectionFactory;

    private readonly string _configurationTableName;
    private readonly string _changeRequestTableName;

    public DatabaseKeeper(IConfiguration configuration)
    {
        _connectionFactory = new ConnectionFactory(configuration["DB_CONNECTION_STRING"]);
        _configurationTableName = configuration["CONFIGURATION_TABLE_NAME"];
        _changeRequestTableName = configuration["CHANGE_REQUEST_TABLE_NAME"];
        ClearTablesData();
    }

    public void ClearTablesData()
    {
        List<Task> tasks = new List<Task>
        {
            ClearTablesData(_configurationTableName),
            ClearTablesData(_changeRequestTableName)
        };

        Task.WhenAll(tasks);
    }

    private async Task ClearTablesData(string tableName)
    {
        await _connectionFactory.GetConnection().ExecuteAsync($"TRUNCATE {tableName} RESTART IDENTITY CASCADE");
    }
}