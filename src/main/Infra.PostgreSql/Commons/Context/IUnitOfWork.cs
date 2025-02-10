using System;
using System.Data;
using System.Threading.Tasks;
using Infra.PostgreSql.Commons.Repository;
using Infra.PostgreSql.Domains.Models;

namespace Infra.PostgreSql.Commons.Context;

public interface IUnitOfWork : IDisposable
{
    public IDbConnection GetConnection();
    public IDbConnection GetWritableConnection();
    public IDbTransaction GetTransaction();
    
    void Invoke(Func<Task> method);
    T Invoke<T>(Func<Task<T>> method);
    
    Task InvokeAsync(Func<Task> method);
    Task<T> InvokeAsync<T>(Func<Task<T>> method);
    
    IRepository<TransactionEntity> Transactions { get; }
}