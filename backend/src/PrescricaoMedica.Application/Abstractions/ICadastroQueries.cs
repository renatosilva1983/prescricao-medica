using PrescricaoMedica.Application.Cadastros;

namespace PrescricaoMedica.Application.Abstractions;

/// <summary>Leitura dos cadastros pré-populados, consultáveis por OData.</summary>
public interface ICadastroQueries
{
    IQueryable<PacienteDto> Pacientes();

    IQueryable<MedicamentoDto> Medicamentos();
}
