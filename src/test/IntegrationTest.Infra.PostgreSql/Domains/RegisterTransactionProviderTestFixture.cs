using Core.Commons.Paging;
using Infra.PostgreSql.Commons.Context;
using Infra.PostgreSql.Commons.Filter;
using Infra.PostgreSql.Domains.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IntegrationTest.Infra.PostgreSql.Commons;
using Xunit;

namespace IntegrationTest.Infra.PostgreSql.Domains.Providers;

[Collection("Database collection")]
public sealed class RegisterTransactionProviderTestFixture : IDisposable
{
    public TransactionEntity TransactionEntity { get; private set; }

    private readonly IUnitOfWork _unitOfWork;

    ~RegisterTransactionProviderTestFixture() => Dispose();

    public RegisterTransactionProviderTestFixture(Context context)
    {
        _unitOfWork = context.ServiceResolver.Resolve<IUnitOfWork>();

        _unitOfWork.Invoke(async () =>
        {
            TransactionEntity = await _unitOfWork.Transactions.AddAsync(new TransactionEntity
            {
                Description = "Initial transaction",
                Amount = 100.50m,
                Type = "CREDIT",
                Status = "ACTIVE",
                Date = new DateTime(2023, 09, 01, 10, 00, 00, DateTimeKind.Local).ToUniversalTime(),
                CreatedAt = new DateTime(2023, 09, 01, 10, 00, 00, DateTimeKind.Local).ToUniversalTime(),
                UpdatedAt = new DateTime(2023, 09, 01, 10, 30, 00, DateTimeKind.Local).ToUniversalTime()
            });
        });
    }

    public void Dispose()
    {
        _unitOfWork.Invoke(async () =>
        {
            PageRequest pageRequest = PageRequest.Of(0, 10);
            FilterDefinition filterDefinition = new(nameof(TransactionEntity.Id), SqlOperator.Equal, TransactionEntity.Id);
            Page<TransactionEntity> page = await _unitOfWork.Transactions.FindAsync(filterDefinition, pageRequest);

            List<Task> tasks = new List<Task>();
            foreach (TransactionEntity transactionEntity in page.Content)
            {
                tasks.Add(_unitOfWork.Transactions.RemoveAsync(transactionEntity.Id));
            }
            Task.WaitAll(tasks.ToArray());
        });

        GC.SuppressFinalize(this);
    }
}
