using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Core.Commons;
using Infra.PostgreSql.Commons.Context;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace IntegrationTest.Infra.PostgreSql.Commons;

public sealed class ServiceResolver : IServiceResolver
{
    private IUnitOfWork _unitOfWork;
    private readonly string _connectionString;

    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public ServiceResolver(IConfiguration configuration)
    {
        _connectionString = configuration["DB_CONNECTION_STRING"];
        AddServices();
    }

    [ExcludeFromCodeCoverage]
    public T Resolve<T>()
    {
        Type type = typeof(T);

        if (_services.TryGetValue(type, out object value)) return (T) value;

        string message = $"Could not find the type {type} in {nameof(ServiceResolver)}";
        throw new ArgumentNullException(message, (Exception) null);
    }

    private void AddServices()
    {
        _services.Add(typeof(IUnitOfWork), GetUnitOfWork());
    }

    private IUnitOfWork GetUnitOfWork()
    {
        if (_unitOfWork != null)
        {
            return _unitOfWork;
        }
        else
        {
            _unitOfWork = new UnitOfWork(GetDbContext());
            return _unitOfWork;
        }
    }

    private DbContext GetDbContext()
    {
        return new DbContext(GetDbDataSource());
    }

    private DbDataSource GetDbDataSource()
    {
        NpgsqlDataSourceBuilder dataSourceBuilder = new NpgsqlDataSourceBuilder(_connectionString);
        return dataSourceBuilder.Build();
    }
}