using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PrescricaoMedica.Infrastructure.Persistence;

/// <summary>O /health só responde saudável se a API consegue falar com o MySQL.</summary>
internal sealed class MySqlHealthCheck(PrescricaoDbContext dbContext) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default) =>
        await dbContext.Database.CanConnectAsync(cancellationToken)
            ? HealthCheckResult.Healthy()
            : HealthCheckResult.Unhealthy("Sem conexão com o MySQL.");
}
