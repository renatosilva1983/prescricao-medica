using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.Application.Abstractions;

/// <summary>
/// Lado de leitura (CQRS): devolve DTOs prontos para a tela, sem passar pelo agregado.
/// A listagem é um <see cref="IQueryable{T}"/>: a API aplica OData e o EF Core traduz tudo
/// para uma consulta SQL, sem que a Application conheça OData ou o banco.
/// </summary>
public interface IPrescricaoQueries
{
    IQueryable<PrescricaoResumoDto> Resumos();

    Task<PrescricaoDetalheDto?> ObterDetalheAsync(int id, CancellationToken cancellationToken);
}
