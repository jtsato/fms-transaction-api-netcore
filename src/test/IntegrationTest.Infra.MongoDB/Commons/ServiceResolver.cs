using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Commons;
using Infra.MongoDB.Commons.Connection;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest.Infra.MongoDB.Commons;

public sealed class ServiceResolver : IServiceResolver
{
    private IConnectionFactory _connectionFactory;

    private string _databaseName;
    private string _transactionCollectionName;
    private string _transactionSequenceCollectionName;

    //private IRepository<PropertyEntity> _transactionRepository;
    //private ISequenceRepository<PropertySequence> _transactionSequenceRepository;

    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public ServiceResolver(IConfiguration configuration)
    {
        LoadEnvironmentVariables(configuration);
        // AddServices();
    }

    [ExcludeFromCodeCoverage]
    public T Resolve<T>()
    {
        Type type = typeof(T);

        if (_services.TryGetValue(type, out object value)) return (T) value;

        string message = $"Could not find the type {type} in {nameof(ServiceResolver)}";
        throw new ArgumentNullException(message, (Exception) null);
    }

    private void LoadEnvironmentVariables(IConfiguration configuration)
    {
        _connectionFactory = new ConnectionFactory(configuration["MONGODB_URL"]);
        _databaseName = configuration["MONGODB_DATABASE"];
        _transactionCollectionName = configuration["TRANSACTION_COLLECTION_NAME"];
        _transactionSequenceCollectionName = configuration["TRANSACTION_SEQUENCE_COLLECTION_NAME"];
    }

    /*
    private void AddServices()
    {
        _services.Add(typeof(IRepository<PropertyEntity>), GetPropertyRepository());
        _services.Add(typeof(ISequenceRepository<PropertySequence>), GetPropertySequenceRepository());

        _services.Add(typeof(IGetPropertyByUuidGateway), GetPropertyByUuidGateway());
        _services.Add(typeof(ISearchPropertiesGateway), GetSearchPropertiesGateway());
        _services.Add(typeof(IRegisterPropertyGateway), GetRegisterPropertyGateway());
    }

    private IRepository<PropertyEntity> GetPropertyRepository()
    {
        return _transactionRepository ??=
            new PropertyRepository(_connectionFactory, _databaseName, _transactionCollectionName);
    }

    private ISequenceRepository<PropertySequence> GetPropertySequenceRepository()
    {
        return _transactionSequenceRepository ??=
            new PropertySequenceRepository(_connectionFactory, _databaseName, _transactionSequenceCollectionName);
    }

    private ISearchPropertiesGateway GetSearchPropertiesGateway()
    {
        return new SearchPropertiesProvider(GetPropertyRepository());
    }

    private IGetPropertyByUuidGateway GetPropertyByUuidGateway()
    {
        return new GetPropertyByUuidProvider(GetPropertyRepository());
    }

    private IRegisterPropertyGateway GetRegisterPropertyGateway()
    {
        return new RegisterPropertyProvider(GetPropertyRepository(), GetPropertySequenceRepository());
    }
    */
}