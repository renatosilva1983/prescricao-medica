namespace PrescricaoMedica.Domain.Prescricoes;

/// <summary>
/// Dados de um medicamento prescrito (ECF.10.03 d), recebidos pelo agregado para criar o item.
/// </summary>
public sealed record DadosItemPrescricao(
    int? MedicamentoId,
    string? NomeMedicamentoLivre,
    string? FormaFarmaceutica,
    decimal DoseQuantidade,
    string? DoseUnidade,
    string? Frequencia,
    string? ViaAdministracao,
    DateOnly DataInicio,
    int? DuracaoDias,
    bool UsoContinuo,
    DateOnly? DataLimiteUso,
    string? Observacao);
