using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Domain.Medicamentos;
using PrescricaoMedica.Domain.Pacientes;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Infrastructure.Persistence;

/// <summary>
/// Mapeia as tabelas criadas pelos scripts de database/ (sem migrations, ADR-004).
/// O DbContext já é uma unidade de trabalho: implementa <see cref="IUnitOfWork"/>.
/// </summary>
public sealed class PrescricaoDbContext(DbContextOptions<PrescricaoDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Prescricao> Prescricoes => Set<Prescricao>();

    public DbSet<ItemPrescricao> ItensPrescricao => Set<ItemPrescricao>();

    public DbSet<Paciente> Pacientes => Set<Paciente>();

    public DbSet<Medicamento> Medicamentos => Set<Medicamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        // Mapeamento fora das entidades (IEntityTypeConfiguration): o Domain não conhece o EF Core.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrescricaoDbContext).Assembly);
}
