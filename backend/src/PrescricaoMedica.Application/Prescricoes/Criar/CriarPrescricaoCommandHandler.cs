using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Prescricoes.Criar;

/// <summary>
/// Caso de uso: emitir uma prescrição. Confere o que depende do banco (paciente e medicamentos)
/// e deixa o restante das regras para o agregado.
/// </summary>
public sealed class CriarPrescricaoCommandHandler(
    IPrescricaoRepository prescricoes,
    IPacienteRepository pacientes,
    IMedicamentoRepository medicamentos,
    IUnitOfWork unitOfWork,
    TimeProvider timeProvider)
    : ICommandHandler<CriarPrescricaoCommand, int>
{
    public async Task<Result<int>> Handle(CriarPrescricaoCommand command, CancellationToken cancellationToken)
    {
        if (!await pacientes.ExisteAsync(command.PacienteId, cancellationToken))
        {
            return Result.Failure<int>(PrescricaoErrors.PacienteNaoEncontrado(command.PacienteId));
        }

        command.GarantirConteudoDoTipo();
        await command.GarantirMedicamentosCadastradosAsync(medicamentos, cancellationToken);

        var prescricao = command.Emitir(command.PacienteId, timeProvider.GetUtcNow().UtcDateTime);

        prescricoes.Adicionar(prescricao);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(prescricao.Id);
    }
}
