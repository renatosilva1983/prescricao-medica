using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;

namespace PrescricaoMedica.Infrastructure.Persistence.Repositories;

internal sealed class MedicamentoRepository(PrescricaoDbContext dbContext) : IMedicamentoRepository
{
    public async Task<IReadOnlyCollection<int>> ListarIdsInexistentesAsync(
        IReadOnlyCollection<int> ids, CancellationToken cancellationToken)
    {
        var existentes = await dbContext.Medicamentos
            .Where(medicamento => ids.Contains(medicamento.Id))
            .Select(medicamento => medicamento.Id)
            .ToListAsync(cancellationToken);

        return ids.Except(existentes).ToList();
    }
}
