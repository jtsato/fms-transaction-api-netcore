using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Commons;
using EntryPoint.WebApi.Commons;
using EntryPoint.WebApi.Commons.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EntryPoint.WebApi;

[ExcludeFromCodeCoverage]
public static class Program
{
    private static async Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressInferBindingSourcesForParameters = true;
            options.SuppressModelStateInvalidFilter = true;
        });

        builder.Services.AddHttpLogging(GetHttpLoggingOptions);

        builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GetLanguageActionFilterAttribute>();
                options.Filters.Add<HandleInvalidModelStateActionFilterAttribute>();
                options.Filters.Add<ExceptionHandlerFilterAttribute>();
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddEndpointsApiExplorer();
        if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging())
        {
            builder.Services.AddSwaggerGen(ConfigureSwaggerGen);
        }

        builder.Services.AddHealthChecks()
            .AddCheck("Health check", () => HealthCheckResult.Healthy(), tags: new[] {"live", "ready"});

        Dictionary<Type, ServiceLifetime> lifetimeByType= DependencyInjector.ConfigureServices(builder.Services);

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                policy =>
                {
                    policy.WithOrigins
                        (
                            "https://patolar.com.br",
                            "https://www.patolar.com.br",
                            "https://app.patolar.com.br",
                            "https://api.patolar.com.br",
                            "https://patolar-dev.flutterflow.app",
                            "https://app.flutterflow.io",
                            "https://ff-debug-service-frontend-free-ygxkweukma-uc.a.run.app",
                            "https://ff-debug-service-frontend-pro-ygxkweukma-uc.a.run.app",
                            "http://localhost:8000"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });

        builder.Services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
            {
                "text/plain",
                "application/json",
            });
        });

        builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        builder.Services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.SmallestSize;
        });

        WebApplication app = builder.Build();

        app.UseCors("CorsPolicy");

        if (app.Services.GetService(typeof(IServiceResolver)) is ServiceResolver serviceResolver)
        {
            serviceResolver.Setup(app.Services, lifetimeByType);
        }

        if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
        {
            app.UseSwagger(ConfigureSwagger);
            app.UseSwaggerUI(options =>
            {
                options.RoutePrefix = "api/transactions-search/v1/swagger";
                options.SwaggerEndpoint("/api/transactions-search/v1/api-docs/v1/swagger.yaml", "Properties Search API");
            });
            RewriteOptions rewriteOptions = new RewriteOptions();
            rewriteOptions.AddRedirect("^$", "swagger");
            app.UseRewriter(rewriteOptions);
        }

        app.UseResponseCompression();
        app.MapControllers();
        app.UsePathBase(new PathString("/api/transactions-search"));
        app.UseRouting();

        app.UseWhen(
            httpContext => !httpContext.Request.Path.StartsWithSegments("/health-check"),
            appBuilder => appBuilder.UseHttpLogging()
        );

        app.UseAuthorization();
        app.MapHealthChecks
        (
            "/health-check/live",
            new HealthCheckOptions {Predicate = healthCheck => healthCheck.Tags.Contains("live")}
        );
        app.MapHealthChecks
        (
            "/health-check/ready",
            new HealthCheckOptions {Predicate = healthCheck => healthCheck.Tags.Contains("ready")}
        );

        await app.RunAsync();
    }

    private static void ConfigureSwaggerGen(SwaggerGenOptions options)
    {
        options.EnableAnnotations();

        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Properties Search API",
            Version = "v1",
            Description = "Properties Search API",
        });

        options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Name = "X-Api-Key",
            Type = SecuritySchemeType.ApiKey
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Name = "X-Api-Key",
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "ApiKey"}
                },
                Array.Empty<string>()
            }
        });

        options.OperationFilter<LanguageOperationFilter>();
        options.OperationFilter<CorrelationIdOperationFilter>();
        options.DocInclusionPredicate((_, api) => !string.IsNullOrWhiteSpace(api.GroupName));
        options.TagActionsBy(api => new[] {api.GroupName});

        string[] methodsOrder = {"post", "put", "patch", "delete", "get", "options", "trace"};
        options.OrderActionsBy(apiDesc => $"{Array.IndexOf(methodsOrder, apiDesc.HttpMethod!.ToLower())}_{apiDesc.HttpMethod}");
    }

    private static void ConfigureSwagger(SwaggerOptions options)
    {
        options.RouteTemplate = "api/transactions-search/v1/api-docs/{documentName}/swagger.yaml";

        options.PreSerializeFilters.Add((swagger, httpReq) =>
        {
            swagger.Servers = new List<OpenApiServer>
            {
                new OpenApiServer {Url = $"{httpReq.Scheme}://{httpReq.Host.Value}/api/transactions-search"}
            };
        });
    }

    private static void GetHttpLoggingOptions(HttpLoggingOptions options)
    {
        options.LoggingFields = HttpLoggingFields.RequestPath
                                | HttpLoggingFields.RequestQuery
                                | HttpLoggingFields.RequestMethod
                                | HttpLoggingFields.RequestBody
                                | HttpLoggingFields.ResponseStatusCode
                                | HttpLoggingFields.ResponseHeaders
                                | HttpLoggingFields.RequestHeaders
                                | HttpLoggingFields.ResponseBody;

        options.RequestHeaders.Add("Accept-Language");
        options.ResponseHeaders.Add("Content-Type");
        options.RequestHeaders.Add("X-Correlation-Id");
        options.ResponseHeaders.Add("X-Correlation-Id");
        options.MediaTypeOptions.AddText("application/json");
        options.RequestBodyLogLimit = 4096;
        options.ResponseBodyLogLimit = 4096;
    }
}