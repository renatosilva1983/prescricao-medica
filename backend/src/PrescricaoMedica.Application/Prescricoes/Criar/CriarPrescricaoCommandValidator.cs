using FluentValidation;

namespace PrescricaoMedica.Application.Prescricoes.Criar;

internal sealed class CriarPrescricaoCommandValidator : ConteudoPrescricaoValidator<CriarPrescricaoCommand>
{
    public CriarPrescricaoCommandValidator() =>
        RuleFor(command => command.PacienteId).GreaterThan(0).WithMessage("Informe o paciente.");
}
