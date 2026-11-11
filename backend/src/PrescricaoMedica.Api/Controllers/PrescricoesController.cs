using Microsoft.AspNetCore.Mvc;
using PrescricaoMedica.Api.Contracts;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Prescricoes.Consultas;
using PrescricaoMedica.Application.Prescricoes.Criar;
using PrescricaoMedica.Application.Prescricoes.Excluir;
using PrescricaoMedica.Application.Prescricoes.Revisar;

namespace PrescricaoMedica.Api.Controllers;

/// <summary>
/// CRUD REST da prescrição. Controller fino: monta o comando, chama o handler e traduz o resultado em HTTP.
/// Cada action recebe só o handler que usa ([FromServices]).
/// </summary>
[Route("api/prescricoes")]
public sealed class PrescricoesController : ApiController
{
    [HttpGet("{id:int}")]
    [ProducesResponseType<PrescricaoDetalheDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Obter(
        int id,
        [FromServices] IQueryHandler<ObterPrescricaoQuery, PrescricaoDetalheDto> handler,
        CancellationToken cancellationToken)
    {
        var resultado = await handler.Handle(new ObterPrescricaoQuery(id), cancellationToken);

        return resultado.IsSuccess ? Ok(resultado.Value) : Problem(resultado.Error);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Criar(
        CriarPrescricaoRequest request,
        [FromServices] ICommandHandler<CriarPrescricaoCommand, int> handler,
        CancellationToken cancellationToken)
    {
        var command = new CriarPrescricaoCommand(
            request.PacienteId,
            request.Tipo,
            request.NomeProfissional,
            request.RegistroProfissional,
            request.TextoLivre,
            request.Itens);

        var resultado = await handler.Handle(command, cancellationToken);

        return resultado.IsSuccess
            ? CreatedAtAction(nameof(Obter), new { id = resultado.Value }, new { id = resultado.Value })
            : Problem(resultado.Error);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Revisar(
        int id,
        RevisarPrescricaoRequest request,
        [FromServices] ICommandHandler<RevisarPrescricaoCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new RevisarPrescricaoCommand(
            id,
            request.Tipo,
            request.NomeProfissional,
            request.RegistroProfissional,
            request.TextoLivre,
            request.Itens);

        var resultado = await handler.Handle(command, cancellationToken);

        return resultado.IsSuccess ? NoContent() : Problem(resultado.Error);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(
        int id,
        [FromServices] ICommandHandler<ExcluirPrescricaoCommand> handler,
        CancellationToken cancellationToken)
    {
        var resultado = await handler.Handle(new ExcluirPrescricaoCommand(id), cancellationToken);

        return resultado.IsSuccess ? NoContent() : Problem(resultado.Error);
    }
}
