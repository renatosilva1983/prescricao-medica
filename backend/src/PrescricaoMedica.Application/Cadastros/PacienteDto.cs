namespace PrescricaoMedica.Application.Cadastros;

/// <summary>Paciente do cadastro, para a busca na tela de prescrição.</summary>
public sealed class PacienteDto
{
    public int Id { get; init; }

    public string NomeCompleto { get; init; } = string.Empty;

    public DateOnly DataNascimento { get; init; }

    public string Sexo { get; init; } = string.Empty;
}
