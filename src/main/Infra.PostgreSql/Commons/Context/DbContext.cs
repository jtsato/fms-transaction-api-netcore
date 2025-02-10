using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Dapper;

namespace Infra.PostgreSql.Commons.Context;

// ReSharper disable ClassNeverInstantiated.Global
public sealed class DbContext : IDisposable
{
    private readonly Guid _id;
    private readonly DbDataSource _dataSource;

    private bool _disposed;

    ~DbContext() => Dispose(false);

    [ExcludeFromCodeCoverage]    
    public override string ToString() => $"DbContext: {_id}";

    public DbContext(DbDataSource dataSource)
    {
        _id = Guid.NewGuid();
        _dataSource = dataSource;
        DefaultTypeMap.MatchNamesWithUnderscores = false;
    }

    public DbConnection GetConnection()
    {
        return _dataSource.CreateConnection();
    }
    
    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;
        _disposed = true;
    }
}
