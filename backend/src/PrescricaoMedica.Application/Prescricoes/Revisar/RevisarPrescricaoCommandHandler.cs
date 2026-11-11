using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Prescricoes.Revisar;

/// <summary>Caso de uso: revisar uma prescrição (substitui todo o conteúdo).</summary>
public sealed class RevisarPrescricaoCommandHandler(
    IPrescricaoRepository prescricoes,
    IMedicamentoRepository medicamentos,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<RevisarPrescricaoCommand>
{
    public async Task<Result> Handle(RevisarPrescricaoCommand command, CancellationToken cancellationToken)
    {
        var prescricao = await prescricoes.ObterPorIdAsync(command.Id, cancellationToken);
        if (prescricao is null)
        {
            return Result.Failure(PrescricaoErrors.NaoEncontrada(command.Id));
        }

        command.GarantirConteudoDoTipo();
        await command.GarantirMedicamentosCadastradosAsync(medicamentos, cancellationToken);

        command.Aplicar(prescricao, timeProvider.GetUtcNow().UtcDateTime);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
