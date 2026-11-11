using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrescricaoMedica.Domain.Pacientes;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Infrastructure.Persistence.Configurations;

internal sealed class PrescricaoConfiguration : IEntityTypeConfiguration<Prescricao>
{
    public void Configure(EntityTypeBuilder<Prescricao> builder)
    {
        builder.ToTable("prescricao");
        builder.HasKey(prescricao => prescricao.Id);

        builder.Property(prescricao => prescricao.Id).HasColumnName("id");
        builder.Property(prescricao => prescricao.PacienteId).HasColumnName("paciente_id");
        builder.Property(prescricao => prescricao.Tipo).HasColumnName("tipo").HasConversion<string>().HasMaxLength(20);
        builder.Property(prescricao => prescricao.NomeProfissional).HasColumnName("nome_profissional").HasMaxLength(150);
        builder.Property(prescricao => prescricao.RegistroProfissional).HasColumnName("registro_profissional").HasMaxLength(30);
        builder.Property(prescricao => prescricao.TextoLivre).HasColumnName("texto_livre").HasMaxLength(4000);
        builder.Property(prescricao => prescricao.EmitidaEmUtc).HasColumnName("emitida_em_utc")
            .HasConversion(UtcConverter.Obrigatorio);
        builder.Property(prescricao => prescricao.AtualizadaEmUtc).HasColumnName("atualizada_em_utc")
            .HasConversion(UtcConverter.Opcional);

        builder.HasOne<Paciente>()
            .WithMany()
            .HasForeignKey(prescricao => prescricao.PacienteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Os itens só mudam pela raiz do agregado: o EF Core usa o campo _itens, não a coleção somente leitura.
        builder.HasMany(prescricao => prescricao.Itens)
            .WithOne()
            .HasForeignKey(item => item.PrescricaoId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.Navigation(prescricao => prescricao.Itens).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
