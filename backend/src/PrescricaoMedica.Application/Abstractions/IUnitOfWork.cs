namespace PrescricaoMedica.Application.Abstractions;

/// <summary>Grava, numa transação, tudo o que os repositórios alteraram no caso de uso.</summary>
public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
