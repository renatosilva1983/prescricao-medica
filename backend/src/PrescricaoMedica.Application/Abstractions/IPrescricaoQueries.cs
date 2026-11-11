using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.Application.Abstractions;

/// <summary>
/// Lado de leitura (CQRS): devolve DTOs prontos para a tela, sem passar pelo agregado.
/// </summary>
public interface IPrescricaoQueries
{
    Task<PrescricaoDetalheDto?> ObterDetalheAsync(int id, CancellationToken cancellationToken);
}
