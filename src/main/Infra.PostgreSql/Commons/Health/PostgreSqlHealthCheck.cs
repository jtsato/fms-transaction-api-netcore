using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Infra.PostgreSql.Commons.Connection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Infra.PostgreSql.Commons.Health;

public sealed class PostgreSqlHealthCheck(IConnectionFactory connectionFactory) : IHealthCheck
{
    private readonly IConnectionFactory _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await using DbConnection connection = _connectionFactory.GetConnection();
            await connection.OpenAsync(cancellationToken);

            await using DbCommand command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            object result = await command.ExecuteScalarAsync(cancellationToken);

            return Equals(result, 1)
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("PostgreSQL did not return the expected readiness response.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unavailable.", exception);
        }
    }
}
