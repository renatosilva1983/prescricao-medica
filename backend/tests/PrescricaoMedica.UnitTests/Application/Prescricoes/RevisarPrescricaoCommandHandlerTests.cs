using NSubstitute;
using PrescricaoMedica.Application.Common.Results;
using PrescricaoMedica.Application.Prescricoes.Revisar;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Application.Prescricoes;

public class RevisarPrescricaoCommandHandlerTests
{
    private readonly Cenarios _cenarios = new();

    [Fact]
    public async Task Handle_PrescricaoExistente_AplicaRevisaoEGrava()
    {
        var prescricao = _cenarios.PrescricaoExistente();
        var command = new RevisarPrescricaoCommand(
            Cenarios.PrescricaoId, TipoPrescricao.Estruturada, "Dr. Ricardo Tavares", "CRM-SP 654321",
            null, [Cenarios.ItemDoCadastro]);

        var resultado = await CriarHandler().Handle(command, CancellationToken.None);

        resultado.IsSuccess.ShouldBeTrue();
        prescricao.Tipo.ShouldBe(TipoPrescricao.Estruturada);
        prescricao.NomeProfissional.ShouldBe("Dr. Ricardo Tavares");
        prescricao.AtualizadaEmUtc.ShouldBe(Cenarios.Agora.UtcDateTime);
        await _cenarios.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PrescricaoInexistente_RetornaNaoEncontrado()
    {
        _cenarios.Prescricoes.ObterPorIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns((Prescricao?)null);
        var command = new RevisarPrescricaoCommand(
            999, TipoPrescricao.NaoEstruturada, Cenarios.NomeProfissional, Cenarios.RegistroProfissional, "Texto.", null);

        var resultado = await CriarHandler().Handle(command, CancellationToken.None);

        resultado.Error.Type.ShouldBe(ErrorType.NotFound);
        await _cenarios.UnitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    private RevisarPrescricaoCommandHandler CriarHandler() => new(
        _cenarios.Prescricoes, _cenarios.Medicamentos, _cenarios.UnitOfWork, _cenarios.Relogio);
}
