using NSubstitute;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Application.Common.Results;
using PrescricaoMedica.Application.Prescricoes.Consultas;

namespace PrescricaoMedica.UnitTests.Application.Prescricoes;

public class ObterPrescricaoQueryHandlerTests
{
    private readonly IPrescricaoQueries _consultas = Substitute.For<IPrescricaoQueries>();

    [Fact]
    public async Task Handle_PrescricaoInexistente_RetornaNaoEncontrado()
    {
        _consultas.ObterDetalheAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((PrescricaoDetalheDto?)null);

        var resultado = await new ObterPrescricaoQueryHandler(_consultas)
            .Handle(new ObterPrescricaoQuery(999), CancellationToken.None);

        resultado.IsFailure.ShouldBeTrue();
        resultado.Error.Type.ShouldBe(ErrorType.NotFound);
    }
}
