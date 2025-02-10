using System;
using System.Diagnostics.CodeAnalysis;
using Infra.PostgreSql.Commons.Context;
using Infra.PostgreSql.Commons.Repository;
using Infra.PostgreSql.Domains.Models;

namespace Infra.PostgreSql.Domains.Repositories;

public sealed class TransactionRepository(IUnitOfWork unitOfWork) : Repository<TransactionEntity>(unitOfWork), IDisposable
{
    private bool _disposed;
    
    ~TransactionRepository() => Dispose(false);

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
