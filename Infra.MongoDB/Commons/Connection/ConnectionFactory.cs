using MongoDB.Driver;

namespace Infra.MongoDB.Commons.Connection;

public sealed class ConnectionFactory : IConnectionFactory
{
    private readonly string _connectionString;

    public ConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IMongoClient GetClient()
    {
        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(_connectionString));

        return new MongoClient(settings);
    }

    public IMongoDatabase GetDatabase(string databaseName)
    {
        IMongoClient mongoDbClient = GetClient();

        return mongoDbClient.GetDatabase(databaseName);
    }
}