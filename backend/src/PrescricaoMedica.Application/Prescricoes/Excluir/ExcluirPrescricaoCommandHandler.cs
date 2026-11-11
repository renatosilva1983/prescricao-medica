using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Prescricoes.Excluir;

/// <summary>Caso de uso: excluir uma prescrição; os itens saem junto.</summary>
public sealed class ExcluirPrescricaoCommandHandler(IPrescricaoRepository prescricoes, IUnitOfWork unitOfWork)
    : ICommandHandler<ExcluirPrescricaoCommand>
{
    public async Task<Result> Handle(ExcluirPrescricaoCommand command, CancellationToken cancellationToken)
    {
        var prescricao = await prescricoes.ObterPorIdAsync(command.Id, cancellationToken);
        if (prescricao is null)
        {
            return Result.Failure(PrescricaoErrors.NaoEncontrada(command.Id));
        }

        prescricoes.Remover(prescricao);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
