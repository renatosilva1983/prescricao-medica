using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.Infrastructure.Persistence.Queries;

/// <summary>Leitura sem rastreamento, projetada direto para os DTOs.</summary>
internal sealed class PrescricaoQueries(PrescricaoDbContext dbContext) : IPrescricaoQueries
{
    public IQueryable<PrescricaoResumoDto> Resumos() =>
        from prescricao in dbContext.Prescricoes.AsNoTracking()
        join paciente in dbContext.Pacientes on prescricao.PacienteId equals paciente.Id
        select new PrescricaoResumoDto
        {
            Id = prescricao.Id,
            PacienteId = prescricao.PacienteId,
            NomePaciente = paciente.NomeCompleto,
            Tipo = prescricao.Tipo,
            NomeProfissional = prescricao.NomeProfissional,
            RegistroProfissional = prescricao.RegistroProfissional,
            EmitidaEmUtc = prescricao.EmitidaEmUtc,
            AtualizadaEmUtc = prescricao.AtualizadaEmUtc,
            QuantidadeItens = prescricao.Itens.Count,
        };

    public async Task<PrescricaoDetalheDto?> ObterDetalheAsync(int id, CancellationToken cancellationToken)
    {
        var cabecalho = await (
                from prescricao in dbContext.Prescricoes.AsNoTracking()
                join paciente in dbContext.Pacientes on prescricao.PacienteId equals paciente.Id
                where prescricao.Id == id
                select new
                {
                    prescricao.Id,
                    prescricao.PacienteId,
                    NomePaciente = paciente.NomeCompleto,
                    prescricao.Tipo,
                    prescricao.NomeProfissional,
                    prescricao.RegistroProfissional,
                    prescricao.TextoLivre,
                    prescricao.EmitidaEmUtc,
                    prescricao.AtualizadaEmUtc,
                })
            .SingleOrDefaultAsync(cancellationToken);

        if (cabecalho is null)
        {
            return null;
        }

        var itens = await (
                from item in dbContext.ItensPrescricao.AsNoTracking()
                where item.PrescricaoId == id
                join medicamento in dbContext.Medicamentos on item.MedicamentoId equals (int?)medicamento.Id into cadastro
                from medicamento in cadastro.DefaultIfEmpty()
                orderby item.Id
                select new ItemPrescricaoDto(
                    item.Id,
                    item.MedicamentoId,
                    medicamento != null ? medicamento.Nome + " " + medicamento.Concentracao : item.NomeMedicamentoLivre!,
                    medicamento != null,
                    item.FormaFarmaceutica,
                    item.DoseQuantidade,
                    item.DoseUnidade,
                    item.Frequencia,
                    item.ViaAdministracao,
                    item.DataInicio,
                    item.DuracaoDias,
                    item.UsoContinuo,
                    item.DataLimiteUso,
                    item.Observacao))
            .ToListAsync(cancellationToken);

        return new PrescricaoDetalheDto(
            cabecalho.Id,
            cabecalho.PacienteId,
            cabecalho.NomePaciente,
            cabecalho.Tipo,
            cabecalho.NomeProfissional,
            cabecalho.RegistroProfissional,
            cabecalho.TextoLivre,
            cabecalho.EmitidaEmUtc,
            cabecalho.AtualizadaEmUtc,
            itens);
    }
}
