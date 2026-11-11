using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Prescricoes.Consultas;

/// <summary>Caso de uso: visualizar uma prescrição.</summary>
public sealed class ObterPrescricaoQueryHandler(IPrescricaoQueries consultas)
    : IQueryHandler<ObterPrescricaoQuery, PrescricaoDetalheDto>
{
    public async Task<Result<PrescricaoDetalheDto>> Handle(ObterPrescricaoQuery query, CancellationToken cancellationToken)
    {
        var detalhe = await consultas.ObterDetalheAsync(query.Id, cancellationToken);

        return detalhe is null
            ? Result.Failure<PrescricaoDetalheDto>(PrescricaoErrors.NaoEncontrada(query.Id))
            : Result.Success(detalhe);
    }
}
