using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Commons;
using EntryPoint.WebApi.Commons;
using EntryPoint.WebApi.Commons.Exceptions;
using EntryPoint.WebApi.Commons.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace EntryPoint.WebApi;

[ExcludeFromCodeCoverage]
public static class DependencyInjector
{
    private static readonly string ConnectionString =
        Environment.GetEnvironmentVariable("MONGODB_URL") ?? string.Empty;

    private static readonly string DatabaseName =
        Environment.GetEnvironmentVariable("MONGODB_DATABASE") ?? string.Empty;

    private static readonly string CollectionName =
        Environment.GetEnvironmentVariable("TRANSACTION_COLLECTION_NAME") ?? string.Empty;

    public static Dictionary<Type, ServiceLifetime> ConfigureServices(IServiceCollection services)
    {
        AddSharedServices(services);
        AddEntryPointServices(services);
        // AddCoreServices(services);
        // AddInfrastructureServices(services, new ConnectionFactory(ConnectionString));

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
        //services.AddSingleton<ISearchTransactionsController, SearchTransactionsController>();
        //services.AddSingleton<IGetPropertyByUuidController, GetPropertyByUuidController>();
    }

    /*
    private static void AddCoreServices(IServiceCollection services)
    {
        services.AddSingleton<ISearchTransactionsUseCase, SearchTransactionsUseCase>();
        services.AddSingleton<IGetPropertyByUuidUseCase, GetPropertyByUuidUseCase>();
    }

    private static void AddInfrastructureServices(IServiceCollection services, IConnectionFactory connectionFactory)
    {
        services.AddSingleton<ISearchTransactionsGateway, SearchTransactionsProvider>();
        services.AddSingleton<IGetPropertyByUuidGateway, GetPropertyByUuidProvider>();
        services.AddSingleton<IRepository<PropertyEntity>>(_ => new PropertyRepository(connectionFactory, DatabaseName, CollectionName));
    }
    */

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