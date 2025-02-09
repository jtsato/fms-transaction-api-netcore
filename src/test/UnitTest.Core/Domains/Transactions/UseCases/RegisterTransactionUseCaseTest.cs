using System;
using System.Diagnostics.CodeAnalysis;
using Core.Commons;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.UseCases;
using Moq;
using UnitTest.Core.Commons;
using Xunit.Abstractions;

namespace UnitTest.Core.Domains.Transactions.UseCases;

[ExcludeFromCodeCoverage]
public sealed class RegisterTransactionUseCaseTest : IDisposable
{
    private readonly ITestOutputHelper _outputHelper;
    private readonly Mock<IRegisterTransactionGateway> _registerTransactionGateway;
    private readonly Mock<IGetDateTime> _getDateTime;
    private readonly IRegisterTransactionUseCase _useCase;
    
    public RegisterTransactionUseCaseTest(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
        _registerTransactionGateway = new Mock<IRegisterTransactionGateway>();
        _getDateTime = new Mock<IGetDateTime>();

        ServiceResolverMocker serviceResolverMocker = new ServiceResolverMocker()
            .AddService(_registerTransactionGateway.Object)
            .AddService(_getDateTime.Object);

        _useCase = new RegisterTransactionUseCase(serviceResolverMocker.Object);        
    }
    
    private bool _disposed;

    ~RegisterTransactionUseCaseTest()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        _registerTransactionGateway.VerifyAll();
        _getDateTime.VerifyAll();
        Dispose(true);
        _outputHelper.WriteLine($"{nameof(RegisterTransactionUseCaseTest)} disposed.");
        GC.SuppressFinalize(this);
    }

    [ExcludeFromCodeCoverage]
    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing) return;
        _disposed = true;
    }    
}