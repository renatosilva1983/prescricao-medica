using FluentValidation;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Common.Behaviors;

/// <summary>
/// Decorator: valida o formato da entrada (FluentValidation) antes de chamar o handler.
/// O handler fica só com a orquestração do caso de uso. Interno: fica fora da varredura de handlers.
/// </summary>
internal static class ValidationDecorator
{
    public sealed class CommandHandler<TCommand, TResponse>(
        ICommandHandler<TCommand, TResponse> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var error = await ValidateAsync(command, validators, cancellationToken);

            return error is null
                ? await inner.Handle(command, cancellationToken)
                : Result.Failure<TResponse>(error);
        }
    }

    public sealed class CommandBaseHandler<TCommand>(
        ICommandHandler<TCommand> inner,
        IEnumerable<IValidator<TCommand>> validators)
        : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            var error = await ValidateAsync(command, validators, cancellationToken);

            return error is null
                ? await inner.Handle(command, cancellationToken)
                : Result.Failure(error);
        }
    }

    public sealed class QueryHandler<TQuery, TResponse>(
        IQueryHandler<TQuery, TResponse> inner,
        IEnumerable<IValidator<TQuery>> validators)
        : IQueryHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
        public async Task<Result<TResponse>> Handle(TQuery query, CancellationToken cancellationToken)
        {
            var error = await ValidateAsync(query, validators, cancellationToken);

            return error is null
                ? await inner.Handle(query, cancellationToken)
                : Result.Failure<TResponse>(error);
        }
    }

    private static async Task<ValidationError?> ValidateAsync<TRequest>(
        TRequest request,
        IEnumerable<IValidator<TRequest>> validators,
        CancellationToken cancellationToken)
    {
        var validatorList = validators.ToArray();
        if (validatorList.Length == 0)
        {
            return null;
        }

        var context = new ValidationContext<TRequest>(request);
        var results = await Task.WhenAll(
            validatorList.Select(validator => validator.ValidateAsync(context, cancellationToken)));

        var errors = results
            .SelectMany(result => result.Errors)
            .GroupBy(failure => failure.PropertyName, failure => failure.ErrorMessage)
            .ToDictionary(group => group.Key, group => group.Distinct().ToArray());

        return errors.Count == 0 ? null : new ValidationError(errors);
    }
}
