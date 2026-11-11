using NSubstitute;
using PrescricaoMedica.Application.Common.Results;
using PrescricaoMedica.Application.Prescricoes.Excluir;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Application.Prescricoes;

public class ExcluirPrescricaoCommandHandlerTests
{
    private readonly Cenarios _cenarios = new();

    [Fact]
    public async Task Handle_PrescricaoExistente_RemoveEGrava()
    {
        var prescricao = _cenarios.PrescricaoExistente();

        var resultado = await CriarHandler().Handle(new ExcluirPrescricaoCommand(Cenarios.PrescricaoId), CancellationToken.None);

        resultado.IsSuccess.ShouldBeTrue();
        _cenarios.Prescricoes.Received(1).Remover(prescricao);
        await _cenarios.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PrescricaoInexistente_RetornaNaoEncontrado()
    {
        _cenarios.Prescricoes.ObterPorIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Prescricao?)null);

        var resultado = await CriarHandler().Handle(new ExcluirPrescricaoCommand(999), CancellationToken.None);

        resultado.Error.Type.ShouldBe(ErrorType.NotFound);
        _cenarios.Prescricoes.DidNotReceiveWithAnyArgs().Remover(default!);
    }

    private ExcluirPrescricaoCommandHandler CriarHandler() => new(_cenarios.Prescricoes, _cenarios.UnitOfWork);
}
