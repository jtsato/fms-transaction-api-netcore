using System;
using System.Data;
using System.Threading.Tasks;

namespace Infra.PostgreSQL.Commons.Context;

public interface IUnitOfWork : IDisposable
{
    public IDbConnection GetConnection();
    public IDbConnection GetWritableConnection();
    public IDbTransaction GetTransaction();
    
    void Invoke(Func<Task> method);
    T Invoke<T>(Func<Task<T>> method);
    
    Task InvokeAsync(Func<Task> method);
    Task<T> InvokeAsync<T>(Func<Task<T>> method);
    
    // IRepository<ConfigurationEntity> Configurations { get; }
    // IRepository<ChangeRequestEntity> ChangeRequests { get; }
}