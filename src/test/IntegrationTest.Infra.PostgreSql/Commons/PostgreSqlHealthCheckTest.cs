using System.Threading.Tasks;
using Infra.PostgreSql.Commons.Connection;
using Infra.PostgreSql.Commons.Health;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Xunit;

namespace IntegrationTest.Infra.PostgreSql.Commons;

[Collection("Database collection")]
public sealed class PostgreSqlHealthCheckTest(Context context)
{
    [Trait("Category", "PostgreSQL integration tests")]
    [Fact(DisplayName = "Successful to report PostgreSQL as healthy")]
    public async Task SuccessfulToReportPostgreSqlAsHealthy()
    {
        // Arrange
        PostgreSqlHealthCheck healthCheck = new PostgreSqlHealthCheck(new ConnectionFactory(context.ConnectionString));

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.Exception is null, result.Exception?.ToString());
        Assert.Equal(HealthStatus.Healthy, result.Status);
    }

    [Trait("Category", "PostgreSQL integration tests")]
    [Fact(DisplayName = "Successful to report PostgreSQL as unhealthy when it cannot be reached")]
    public async Task SuccessfulToReportPostgreSqlAsUnhealthyWhenItCannotBeReached()
    {
        // Arrange
        PostgreSqlHealthCheck healthCheck = new PostgreSqlHealthCheck(new ConnectionFactory(
            "Host=127.0.0.1;Port=1;Database=unavailable;Username=unavailable;Password=unavailable;Timeout=1"));

        // Act
        HealthCheckResult result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.NotNull(result.Exception);
    }
}
