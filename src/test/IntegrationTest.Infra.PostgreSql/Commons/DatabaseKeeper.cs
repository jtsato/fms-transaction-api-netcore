using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Infra.PostgreSql.Commons.Connection;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest.Infra.PostgreSql.Commons;

public sealed class DatabaseKeeper
{
    private readonly IConnectionFactory _connectionFactory;

    private readonly string _configurationTableName;
    private readonly string _TransactionTableName;

    public DatabaseKeeper(IConfiguration configuration)
    {
        _connectionFactory = new ConnectionFactory(configuration["DB_CONNECTION_STRING"]);
        _configurationTableName = configuration["CONFIGURATION_TABLE_NAME"];
        _TransactionTableName = configuration["TRANSACTION_TABLE_NAME"];
        ClearTablesData();
    }

    public void ClearTablesData()
    {
        List<Task> tasks = new List<Task>
        {
            ClearTablesData(_configurationTableName),
            ClearTablesData(_TransactionTableName)
        };

        Task.WhenAll(tasks);
    }

    private async Task ClearTablesData(string tableName)
    {
        await _connectionFactory.GetConnection().ExecuteAsync($"TRUNCATE {tableName} RESTART IDENTITY CASCADE");
    }
}