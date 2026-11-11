using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescricaoMedica.Domain.Medicamentos;

namespace PrescricaoMedica.Infrastructure.Persistence.Configurations;

internal sealed class MedicamentoConfiguration : IEntityTypeConfiguration<Medicamento>
{
    public void Configure(EntityTypeBuilder<Medicamento> builder)
    {
        builder.ToTable("medicamento");
        builder.HasKey(medicamento => medicamento.Id);

        builder.Property(medicamento => medicamento.Id).HasColumnName("id");
        builder.Property(medicamento => medicamento.Nome).HasColumnName("nome").HasMaxLength(150);
        builder.Property(medicamento => medicamento.PrincipioAtivo).HasColumnName("principio_ativo").HasMaxLength(150);
        builder.Property(medicamento => medicamento.FormaFarmaceutica).HasColumnName("forma_farmaceutica").HasMaxLength(60);
        builder.Property(medicamento => medicamento.Concentracao).HasColumnName("concentracao").HasMaxLength(60);
    }
}
