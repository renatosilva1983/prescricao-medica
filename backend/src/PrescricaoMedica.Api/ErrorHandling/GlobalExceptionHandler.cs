using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Api.ErrorHandling;

/// <summary>
/// Converte exceções em ProblemDetails, sem expor detalhes internos:
/// regra de negócio violada vira 422 com o requisito SBIS e o código da regra; o resto vira 500.
/// </summary>
internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problem = exception switch
        {
            RegraNegocioException regraViolada => new ProblemDetails
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = "Regra de negócio violada",
                Detail = regraViolada.Message,
                Extensions =
                {
                    ["regra"] = regraViolada.Regra.Requisito,
                    ["codigo"] = regraViolada.Regra.Codigo,
                },
            },
            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Erro inesperado",
                Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde.",
            },
        };

        if (problem.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Erro não tratado em {Path}", httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = problem.Status!.Value;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception,
        });
    }
}
