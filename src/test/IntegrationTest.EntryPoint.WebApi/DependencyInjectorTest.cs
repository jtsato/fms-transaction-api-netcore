using System;
using System.Linq;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.UseCases;
using EntryPoint.WebApi;
using EntryPoint.WebApi.Domains.Commons;
using Infra.PostgreSql.Commons.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTest.EntryPoint.WebApi;

public sealed class DependencyInjectorTest
{
    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Successful to configure the transaction dependency graph")]
    public void SuccessfulToConfigureTheTransactionDependencyGraph()
    {
        // Arrange
        ServiceCollection services = new ServiceCollection();
        ConfigurationManager configuration = new ConfigurationManager
        {
            ["DB_CONNECTION_STRING"] = "Host=localhost;Port=5432;Database=test;Username=test;Password=test"
        };

        // Act
        _ = DependencyInjector.ConfigureServices(services, configuration);

        // Assert
        Assert.Contains(services, service => service.ServiceType == typeof(IRegisterTransactionController));
        Assert.Contains(services, service => service.ServiceType == typeof(IRegisterTransactionUseCase));
        Assert.Contains(services, service => service.ServiceType == typeof(IRegisterTransactionGateway));
        Assert.Contains(services, service => service.ServiceType == typeof(IUnitOfWork));
        Assert.Equal(ServiceLifetime.Scoped, services.Single(service => service.ServiceType == typeof(IRegisterTransactionController)).Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, services.Single(service => service.ServiceType == typeof(IRegisterTransactionUseCase)).Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, services.Single(service => service.ServiceType == typeof(IRegisterTransactionGateway)).Lifetime);
        Assert.Equal(ServiceLifetime.Scoped, services.Single(service => service.ServiceType == typeof(IUnitOfWork)).Lifetime);
    }

    [Trait("Category", "WebApi Collection [NoContext]")]
    [Fact(DisplayName = "Fail to configure the transaction dependency graph without a database connection string")]
    public void FailToConfigureTheTransactionDependencyGraphWithoutADatabaseConnectionString()
    {
        // Arrange
        ServiceCollection services = new ServiceCollection();
        ConfigurationManager configuration = new ConfigurationManager();

        // Act
        // Assert
        ArgumentException exception = Assert.Throws<ArgumentException>(() =>
            DependencyInjector.ConfigureServices(services, configuration));

        Assert.Contains("DB_CONNECTION_STRING", exception.Message);
    }
}
