namespace PrescricaoMedica.Application.Abstractions;

/// <summary>Consulta ao cadastro de pacientes (somente leitura).</summary>
public interface IPacienteRepository
{
    Task<bool> ExisteAsync(int id, CancellationToken cancellationToken);
}
