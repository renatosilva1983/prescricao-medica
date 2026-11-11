using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using PrescricaoMedica.Application.Cadastros;
using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.Api.OData;

/// <summary>Modelo EDM: os conjuntos consultáveis em /odata, todos sobre DTOs de leitura.</summary>
internal static class ODataModel
{
    public const string Prefixo = "odata";
    public const int TamanhoPagina = 50;
    public const int TopMaximo = 100;

    public static IEdmModel Criar()
    {
        var builder = new ODataConventionModelBuilder();
        builder.EntitySet<PrescricaoResumoDto>("Prescricoes");
        builder.EntitySet<PacienteDto>("Pacientes");
        builder.EntitySet<MedicamentoDto>("Medicamentos");

        // Mesmo padrão do JSON da API REST: propriedades em camelCase ($filter=nomePaciente eq ...).
        builder.EnableLowerCamelCase();

        return builder.GetEdmModel();
    }
}
