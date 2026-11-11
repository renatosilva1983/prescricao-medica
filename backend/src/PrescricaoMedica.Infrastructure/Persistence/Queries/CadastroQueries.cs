using Microsoft.EntityFrameworkCore;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Cadastros;

namespace PrescricaoMedica.Infrastructure.Persistence.Queries;

internal sealed class CadastroQueries(PrescricaoDbContext dbContext) : ICadastroQueries
{
    public IQueryable<PacienteDto> Pacientes() =>
        dbContext.Pacientes.AsNoTracking().Select(paciente => new PacienteDto
        {
            Id = paciente.Id,
            NomeCompleto = paciente.NomeCompleto,
            DataNascimento = paciente.DataNascimento,
            Sexo = paciente.Sexo,
        });

    public IQueryable<MedicamentoDto> Medicamentos() =>
        dbContext.Medicamentos.AsNoTracking().Select(medicamento => new MedicamentoDto
        {
            Id = medicamento.Id,
            Nome = medicamento.Nome,
            PrincipioAtivo = medicamento.PrincipioAtivo,
            FormaFarmaceutica = medicamento.FormaFarmaceutica,
            Concentracao = medicamento.Concentracao,
        });
}
