using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Domain.Pacientes;

/// <summary>Paciente do cadastro pré-populado. Nesta solução, só é consultado.</summary>
public sealed class Paciente : Entity
{
    private Paciente() { }

    public string NomeCompleto { get; private set; } = string.Empty;

    public DateOnly DataNascimento { get; private set; }

    /// <summary>F (feminino), M (masculino) ou I (indeterminado).</summary>
    public string Sexo { get; private set; } = string.Empty;
}
