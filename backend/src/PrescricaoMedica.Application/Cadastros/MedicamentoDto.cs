namespace PrescricaoMedica.Application.Cadastros;

/// <summary>Medicamento da lista de seleção da receita estruturada (ECF.10.03 b).</summary>
public sealed class MedicamentoDto
{
    public int Id { get; init; }

    public string Nome { get; init; } = string.Empty;

    public string PrincipioAtivo { get; init; } = string.Empty;

    public string FormaFarmaceutica { get; init; } = string.Empty;

    public string Concentracao { get; init; } = string.Empty;
}
