namespace PrescricaoMedica.Application.Abstractions;

/// <summary>Consulta ao cadastro de medicamentos (somente leitura).</summary>
public interface IMedicamentoRepository
{
    /// <summary>Dos ids informados, devolve os que não existem no cadastro.</summary>
    Task<IReadOnlyCollection<int>> ListarIdsInexistentesAsync(
        IReadOnlyCollection<int> ids, CancellationToken cancellationToken);
}
