using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Domain.Medicamentos;

/// <summary>
/// Medicamento do cadastro da instituição: a lista de seleção da receita estruturada (ECF.10.03 b).
/// Nesta solução, só é consultado.
/// </summary>
public sealed class Medicamento : Entity
{
    private Medicamento() { }

    public string Nome { get; private set; } = string.Empty;

    public string PrincipioAtivo { get; private set; } = string.Empty;

    public string FormaFarmaceutica { get; private set; } = string.Empty;

    public string Concentracao { get; private set; } = string.Empty;
}
