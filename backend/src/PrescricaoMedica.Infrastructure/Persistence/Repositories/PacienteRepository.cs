using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;

namespace PrescricaoMedica.Infrastructure.Persistence.Repositories;

internal sealed class PacienteRepository(PrescricaoDbContext dbContext) : IPacienteRepository
{
    public Task<bool> ExisteAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Pacientes.AnyAsync(paciente => paciente.Id == id, cancellationToken);
}
