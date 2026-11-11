using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes.Consultas;

/// <summary>Linha da listagem de prescrições, consultável por OData ($filter, $orderby, $select...).</summary>
public sealed class PrescricaoResumoDto
{
    public int Id { get; init; }

    public int PacienteId { get; init; }

    public string NomePaciente { get; init; } = string.Empty;

    public TipoPrescricao Tipo { get; init; }

    public string NomeProfissional { get; init; } = string.Empty;

    public string RegistroProfissional { get; init; } = string.Empty;

    public DateTime EmitidaEmUtc { get; init; }

    public DateTime? AtualizadaEmUtc { get; init; }

    public int QuantidadeItens { get; init; }
}
