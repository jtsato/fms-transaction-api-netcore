using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest.Infra.PostgreSql.Commons;

[ExcludeFromCodeCoverage]
public sealed class Context : IDisposable
{
    private const string EnvTestConfigurationFile = "TEST_CONFIGURATION_FILE";
    private const string DefaultTestConfigurationFile = "test-local.settings.json";

    public ServiceResolver ServiceResolver { get; private set; }
    public string ConnectionString { get; private set; }

    private bool _disposed;

    ~Context() => Dispose(false);

    public Context()
    {
        IConfiguration configuration = InitConfiguration();
        ConnectionString = configuration["DB_CONNECTION_STRING"];

        DockerKeeper dockerKeeper = new DockerKeeper(configuration);
        dockerKeeper.DockerComposeUp();

        DatabaseKeeper databaseKeeper = new DatabaseKeeper(configuration);
        databaseKeeper.ClearTablesData();

        ServiceResolver = new ServiceResolver(configuration);
    }

    private static IConfiguration InitConfiguration()
    {
        string environmentVariable = Environment.GetEnvironmentVariable(EnvTestConfigurationFile)
                                     ?? DefaultTestConfigurationFile;

        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(environmentVariable)
            .Build();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) 
            return;
        _disposed = true;
    }
}