using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Infrastructure.Persistence.Repositories;

internal sealed class PrescricaoRepository(PrescricaoDbContext dbContext) : IPrescricaoRepository
{
    public Task<Prescricao?> ObterPorIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Prescricoes
            .Include(prescricao => prescricao.Itens)
            .SingleOrDefaultAsync(prescricao => prescricao.Id == id, cancellationToken);

    public void Adicionar(Prescricao prescricao) => dbContext.Prescricoes.Add(prescricao);

    // Os itens saem junto, pela exclusão em cascata.
    public void Remover(Prescricao prescricao) => dbContext.Prescricoes.Remove(prescricao);
}
