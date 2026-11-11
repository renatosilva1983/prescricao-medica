using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Cadastros;
using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.Api.OData;

/// <summary>
/// Consultas OData. Cada action devolve um IQueryable: $filter, $orderby, $select, $top, $skip
/// e $count viram uma única consulta SQL. A página tem no máximo 50 linhas.
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public sealed class ConsultasController : ODataController
{
    [HttpGet("odata/Prescricoes")]
    [EnableQuery(PageSize = ODataModel.TamanhoPagina, MaxTop = ODataModel.TopMaximo)]
    public IQueryable<PrescricaoResumoDto> Prescricoes([FromServices] IPrescricaoQueries consultas) =>
        consultas.Resumos();

    [HttpGet("odata/Pacientes")]
    [EnableQuery(PageSize = ODataModel.TamanhoPagina, MaxTop = ODataModel.TopMaximo)]
    public IQueryable<PacienteDto> Pacientes([FromServices] ICadastroQueries consultas) =>
        consultas.Pacientes();

    [HttpGet("odata/Medicamentos")]
    [EnableQuery(PageSize = ODataModel.TamanhoPagina, MaxTop = ODataModel.TopMaximo)]
    public IQueryable<MedicamentoDto> Medicamentos([FromServices] ICadastroQueries consultas) =>
        consultas.Medicamentos();
}
