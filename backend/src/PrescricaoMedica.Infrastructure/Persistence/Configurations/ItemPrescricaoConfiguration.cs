using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescricaoMedica.Domain.Medicamentos;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Infrastructure.Persistence.Configurations;

internal sealed class ItemPrescricaoConfiguration : IEntityTypeConfiguration<ItemPrescricao>
{
    public void Configure(EntityTypeBuilder<ItemPrescricao> builder)
    {
        builder.ToTable("item_prescricao");
        builder.HasKey(item => item.Id);

        builder.Property(item => item.Id).HasColumnName("id");
        builder.Property(item => item.PrescricaoId).HasColumnName("prescricao_id");
        builder.Property(item => item.MedicamentoId).HasColumnName("medicamento_id");
        builder.Property(item => item.NomeMedicamentoLivre).HasColumnName("nome_medicamento_livre").HasMaxLength(150);
        builder.Property(item => item.FormaFarmaceutica).HasColumnName("forma_farmaceutica").HasMaxLength(60);
        builder.Property(item => item.DoseQuantidade).HasColumnName("dose_quantidade").HasPrecision(10, 3);
        builder.Property(item => item.DoseUnidade).HasColumnName("dose_unidade").HasMaxLength(20);
        builder.Property(item => item.Frequencia).HasColumnName("frequencia").HasMaxLength(60);
        builder.Property(item => item.ViaAdministracao).HasColumnName("via_administracao").HasMaxLength(40);
        builder.Property(item => item.DataInicio).HasColumnName("data_inicio");
        builder.Property(item => item.DuracaoDias).HasColumnName("duracao_dias");
        builder.Property(item => item.UsoContinuo).HasColumnName("uso_continuo");
        builder.Property(item => item.DataLimiteUso).HasColumnName("data_limite_uso");
        builder.Property(item => item.Observacao).HasColumnName("observacao").HasMaxLength(500);

        // Opcional: nulo quando o medicamento está fora do cadastro (ECF.10.03 c).
        builder.HasOne<Medicamento>()
            .WithMany()
            .HasForeignKey(item => item.MedicamentoId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
