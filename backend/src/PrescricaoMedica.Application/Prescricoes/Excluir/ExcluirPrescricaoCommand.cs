using PrescricaoMedica.Application.Common.Messaging;

namespace PrescricaoMedica.Application.Prescricoes.Excluir;

public sealed record ExcluirPrescricaoCommand(int Id) : ICommand;
