using System;
using System.Diagnostics.CodeAnalysis;
using Infra.PostgreSQL.Commons.Context;
using Infra.PostgreSQL.Commons.Repository;
using Infra.PostgreSQL.Domains.Models;

namespace Infra.PostgreSQL.Domains.Repositories;

public sealed class ChangeRequestRepository : Repository<ChangeRequestEntity>, IDisposable
{
    private bool _disposed;
    
    ~ChangeRequestRepository() => Dispose(false);
    
    public ChangeRequestRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
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