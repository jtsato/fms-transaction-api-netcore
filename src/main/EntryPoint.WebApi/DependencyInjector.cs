using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Core.Commons;
using Core.Domains.Transactions.Gateways;
using Core.Domains.Transactions.UseCases;
using EntryPoint.WebApi.Commons;
using EntryPoint.WebApi.Commons.Exceptions;
using EntryPoint.WebApi.Commons.Filters;
using EntryPoint.WebApi.Domains.Commons;
using EntryPoint.WebApi.Domains.Transactions.EntryPoints;
using Infra.PostgreSql.Commons.Connection;
using Infra.PostgreSql.Commons.Context;
using Infra.PostgreSql.Domains.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace EntryPoint.WebApi;

[ExcludeFromCodeCoverage]
public static class DependencyInjector
{
    public static Dictionary<Type, ServiceLifetime> ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        ArgumentValidator.CheckNull(services, nameof(services));
        ArgumentValidator.CheckNull(configuration, nameof(configuration));

        AddSharedServices(services);
        AddEntryPointServices(services);
        AddCoreServices(services);
        AddInfrastructureServices(services, configuration);

        return BuildLifetimeByType(services);
    }

    private static void AddSharedServices(IServiceCollection services)
    {
        services.AddSingleton<IServiceResolver, ServiceResolver>();
        services.AddSingleton<IGetDateTime, GetDateTime>();
        services.AddTransient<ILoggerAdapter, LoggerAdapter<ExceptionHandlerFilterAttribute>>();
    }

    private static void AddEntryPointServices(IServiceCollection services)
    {
        services.AddLocalization(options => options.ResourcesPath = "Resources");
        services.AddSingleton<IExceptionHandler, ExceptionHandler>();
        services.AddSingleton<IGetCorrelationId, GetCorrelationId>();
        services.AddScoped<IRegisterTransactionController, RegisterTransactionController>();
    }

    private static void AddCoreServices(IServiceCollection services)
    {
        services.AddScoped<IRegisterTransactionUseCase, RegisterTransactionUseCase>();
    }

    private static void AddInfrastructureServices(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = ArgumentValidator.CheckEmpty(
            configuration["DB_CONNECTION_STRING"],
            "DB_CONNECTION_STRING",
            "The DB_CONNECTION_STRING configuration value is required.");

        services.AddSingleton<DbDataSource>(_ => new NpgsqlDataSourceBuilder(connectionString).Build());
        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory(connectionString));
        services.AddSingleton<DbContext>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRegisterTransactionGateway, RegisterTransactionProvider>();
    }

    private static Dictionary<Type, ServiceLifetime> BuildLifetimeByType(IServiceCollection services)
    {
        Dictionary<Type, ServiceLifetime> lifetimeByType = new Dictionary<Type, ServiceLifetime>();
        foreach (ServiceDescriptor service in services)
        {
            if (service.Lifetime != ServiceLifetime.Singleton) continue;
            if (lifetimeByType.ContainsKey(service.ServiceType)) continue;
            lifetimeByType.Add(service.ServiceType, service.Lifetime);
        }

        return lifetimeByType;
    }
}
