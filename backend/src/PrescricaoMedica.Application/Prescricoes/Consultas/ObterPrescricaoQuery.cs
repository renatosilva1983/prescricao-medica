using PrescricaoMedica.Application.Common.Messaging;

namespace PrescricaoMedica.Application.Prescricoes.Consultas;

public sealed record ObterPrescricaoQuery(int Id) : IQuery<PrescricaoDetalheDto>;
