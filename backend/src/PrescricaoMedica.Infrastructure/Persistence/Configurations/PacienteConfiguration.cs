using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescricaoMedica.Domain.Pacientes;

namespace PrescricaoMedica.Infrastructure.Persistence.Configurations;

internal sealed class PacienteConfiguration : IEntityTypeConfiguration<Paciente>
{
    public void Configure(EntityTypeBuilder<Paciente> builder)
    {
        builder.ToTable("paciente");
        builder.HasKey(paciente => paciente.Id);

        builder.Property(paciente => paciente.Id).HasColumnName("id");
        builder.Property(paciente => paciente.NomeCompleto).HasColumnName("nome_completo").HasMaxLength(150);
        builder.Property(paciente => paciente.DataNascimento).HasColumnName("data_nascimento");
        builder.Property(paciente => paciente.Sexo).HasColumnName("sexo").HasMaxLength(1).IsFixedLength();
    }
}
