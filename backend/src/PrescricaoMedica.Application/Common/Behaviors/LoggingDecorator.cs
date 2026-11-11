using System.Diagnostics;
using Microsoft.Extensions.Logging;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Common.Behaviors;

/// <summary>Decorator: registra início, fim, duração e erro de cada caso de uso.</summary>
internal static class LoggingDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        ILogger<CommandHandler<TCommand, TResponse>> logger)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken) =>
            LogAsync(logger, typeof(TCommand).Name, () => inner.Handle(command, cancellationToken));
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        ILogger<CommandBaseHandler<TCommand>> logger)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public Task<Result> Handle(TCommand command, CancellationToken cancellationToken) =>
            LogAsync(logger, typeof(TCommand).Name, () => inner.Handle(command, cancellationToken));
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        ILogger<QueryHandler<TQuery, TResponse>> logger)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken) =>
            LogAsync(logger, typeof(TQuery).Name, () => inner.Handle(query, cancellationToken));
    }

    private static async Task<TResult> LogAsync<TResult>(ILogger logger, string requestName, Func<Task<TResult>> next)
        where TResult : Result
    {
        logger.LogInformation("Processando {RequestName}", requestName);
        var inicio = Stopwatch.GetTimestamp();

        var result = await next();

        var duracaoMs = Stopwatch.GetElapsedTime(inicio).TotalMilliseconds;
        if (result.IsSuccess)
        {
            logger.LogInformation("{RequestName} concluído em {DuracaoMs:0} ms", requestName, duracaoMs);
        }
        else
        {
            logger.LogWarning(
                "{RequestName} concluído com o erro {ErrorCode} em {DuracaoMs:0} ms",
                requestName,
                result.Error.Code,
                duracaoMs);
        }

        return result;
    }
}
