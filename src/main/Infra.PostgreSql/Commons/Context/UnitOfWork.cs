using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Infra.PostgreSQL.Commons.Context
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly Guid _id;

        private DbContext DbContext { get; set; }

        private IDbConnection _connection;
        private IDbTransaction _transaction;

        // public IRepository<ConfigurationEntity> Configurations { get; }
        // public IRepository<ChangeRequestEntity> ChangeRequests { get; }

        private bool _disposed;

        ~UnitOfWork() => Dispose(false);

        [ExcludeFromCodeCoverage]
        public override string ToString() => $"UnitOfWork: {_id}";

        public UnitOfWork(DbContext dbContext)
        {
            _id = Guid.NewGuid();
            DbContext = dbContext;
            // Configurations = new ConfigurationRepository(this);
            // ChangeRequests = new ChangeRequestRepository(this);
        }

        [ExcludeFromCodeCoverage]
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

        public IDbConnection GetConnection()
        {
            // Return the writable connection if it is open
            // Otherwise, return a read-only connection
            return _connection ?? DbContext.GetConnection();
        }

        public IDbConnection GetWritableConnection()
        {
            // Return the writable connection if it is open
            // Otherwise, throw an exception because we cannot write without a transaction
            if (_connection != null)
            {
                return _connection;
            }
            throw new InvalidOperationException("There is no active transaction");
        }

        public IDbTransaction GetTransaction()
        {
            return _transaction;
        }

        public T Invoke<T>(Func<Task<T>> method)
        {
            return ExecuteInTransaction(method);
        }

        public void Invoke(Func<Task> method)
        {
            ExecuteInTransaction(method);
        }

        public async Task<T> InvokeAsync<T>(Func<Task<T>> method)
        {
            return await Task.Run(() => ExecuteInTransaction(method));
        }

        public async Task InvokeAsync(Func<Task> method)
        {
            await ExecuteInTransactionAsync(async () => await method());
        }
        
        private async Task ExecuteInTransactionAsync(Func<Task> method)
        {
            await Task.Run(() => ExecuteInTransaction(method));
        }

        private void ExecuteInTransaction(Func<Task> method)
        {
            ExecuteInTransaction<object>(async () =>
            {
                await method();
                return 0;
            });
        }

        private T ExecuteInTransaction<T>(Func<Task<T>> method)
        {
            using IDbConnection connection = DbContext.GetConnection();
            connection.Open();

            using IDbTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable);
            _connection = connection;
            _transaction = transaction;

            try
            {
                T result = method().GetAwaiter().GetResult();
                _transaction.Commit();
                return result;
            }
            catch (Exception)
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                CleanupTransaction();
            }
        }

        private void CleanupTransaction()
        {
            _transaction?.Dispose();
            _transaction = null;
            _connection = null;
        }
    }
}
