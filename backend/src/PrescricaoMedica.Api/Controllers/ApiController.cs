using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Api.Controllers;

/// <summary>
/// Base dos controllers: traduz o <see cref="Error"/> do caso de uso em ProblemDetails.
/// Um lugar só decide o status de cada tipo de erro.
/// </summary>
[ApiController]
[Produces("application/json")]
public abstract class ApiController : ControllerBase
{
    protected IActionResult Problem(Error error)
    {
        if (error is ValidationError validationError)
        {
            var modelState = new ModelStateDictionary();
            foreach (var (campo, mensagens) in validationError.Errors)
            {
                foreach (var mensagem in mensagens)
                {
                    modelState.AddModelError(campo, mensagem);
                }
            }

            return ValidationProblem(modelState);
        }

        var statusCode = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status400BadRequest,
        };

        var titulo = error.Type == ErrorType.NotFound ? "Recurso não encontrado" : "Requisição inválida";
        var problem = ProblemDetailsFactory.CreateProblemDetails(
            HttpContext, statusCode: statusCode, title: titulo, detail: error.Description);
        problem.Extensions["codigo"] = error.Code;

        return new ObjectResult(problem) { StatusCode = statusCode };
    }
}
