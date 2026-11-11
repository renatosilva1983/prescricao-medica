using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Abstractions;

/// <summary>Porta de escrita do agregado <see cref="Prescricao"/>.</summary>
public interface IPrescricaoRepository
{
    /// <summary>Obtém a prescrição com os itens, pronta para ser revisada ou excluída.</summary>
    Task<Prescricao?> ObterPorIdAsync(int id, CancellationToken cancellationToken);

    void Adicionar(Prescricao prescricao);

    void Remover(Prescricao prescricao);
}
