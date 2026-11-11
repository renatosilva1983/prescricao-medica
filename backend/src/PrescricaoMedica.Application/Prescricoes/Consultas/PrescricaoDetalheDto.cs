using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes.Consultas;

/// <summary>Visualização de uma prescrição com o nome do paciente e os itens.</summary>
public sealed record PrescricaoDetalheDto(
    int Id,
    int PacienteId,
    string NomePaciente,
    TipoPrescricao Tipo,
    string NomeProfissional,
    string RegistroProfissional,
    string? TextoLivre,
    DateTime EmitidaEmUtc,
    DateTime? AtualizadaEmUtc,
    IReadOnlyList<ItemPrescricaoDto> Itens);

/// <summary>
/// Item da receita estruturada. <c>NomeMedicamento</c> vem do cadastro (nome e concentração)
/// ou do nome livre; <c>MedicamentoCadastrado</c> diz qual das duas origens.
/// </summary>
public sealed record ItemPrescricaoDto(
    int Id,
    int? MedicamentoId,
    string NomeMedicamento,
    bool MedicamentoCadastrado,
    string FormaFarmaceutica,
    decimal DoseQuantidade,
    string DoseUnidade,
    string Frequencia,
    string ViaAdministracao,
    DateOnly DataInicio,
    int? DuracaoDias,
    bool UsoContinuo,
    DateOnly? DataLimiteUso,
    string? Observacao);
