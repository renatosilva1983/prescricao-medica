using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Infrastructure.Persistence;
using PrescricaoMedica.Infrastructure.Persistence.Repositories;

namespace PrescricaoMedica.Infrastructure;

public static class DependencyInjection
{
    private static readonly MySqlServerVersion VersaoServidor = new(new Version(8, 4, 0));

    /// <summary>
    /// Liga as portas da Application às implementações desta camada.
    /// A Api chama só este método: é o único ponto de contato com a Infrastructure.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Options Pattern: sem conexão configurada, a API não sobe (erro na partida, não na primeira requisição).
        services.AddOptions<MySqlOptions>()
            .Bind(configuration.GetSection(MySqlOptions.Secao))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<PrescricaoDbContext>((provider, options) =>
        {
            var mySql = provider.GetRequiredService<IOptions<MySqlOptions>>().Value;
            options.UseMySql(mySql.ConnectionString, VersaoServidor);
        });

        // Scoped: um DbContext por requisição, compartilhado pelos repositórios e pela unidade de trabalho.
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<PrescricaoDbContext>());
        services.AddScoped<IPrescricaoRepository, PrescricaoRepository>();
        services.AddScoped<IPacienteRepository, PacienteRepository>();
        services.AddScoped<IMedicamentoRepository, MedicamentoRepository>();

        services.AddHealthChecks().AddCheck<MySqlHealthCheck>("mysql");

        return services;
    }
}
