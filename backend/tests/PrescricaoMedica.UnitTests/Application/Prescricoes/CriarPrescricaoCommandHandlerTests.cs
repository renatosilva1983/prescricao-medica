using NSubstitute;
using PrescricaoMedica.Application.Common.Results;
using PrescricaoMedica.Application.Prescricoes.Criar;
using PrescricaoMedica.Domain.Common;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Application.Prescricoes;

public class CriarPrescricaoCommandHandlerTests
{
    private readonly Cenarios _cenarios = new();

    [Fact]
    public async Task Handle_ReceitaEstruturadaValida_AdicionaEGrava()
    {
        var command = Estruturada(Cenarios.ItemDoCadastro);

        var resultado = await CriarHandler().Handle(command, CancellationToken.None);

        resultado.IsSuccess.ShouldBeTrue();
        _cenarios.Prescricoes.Received(1).Adicionar(Arg.Is<Prescricao>(prescricao =>
            prescricao.Tipo == TipoPrescricao.Estruturada &&
            prescricao.Itens.Count == 1 &&
            prescricao.EmitidaEmUtc == Cenarios.Agora.UtcDateTime));
        await _cenarios.UnitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PacienteInexistente_RetornaNaoEncontradoSemGravar()
    {
        var command = Estruturada(Cenarios.ItemDoCadastro) with { PacienteId = 999 };

        var resultado = await CriarHandler().Handle(command, CancellationToken.None);

        resultado.IsFailure.ShouldBeTrue();
        resultado.Error.Type.ShouldBe(ErrorType.NotFound);
        _cenarios.Prescricoes.DidNotReceiveWithAnyArgs().Adicionar(default!);
        await _cenarios.UnitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_MedicamentoForaDoCadastro_LancaRegraEcf1003()
    {
        _cenarios.MedicamentosInexistentes(999);
        var command = Estruturada(Cenarios.ItemDoCadastro with { MedicamentoId = 999 });

        var excecao = await Should.ThrowAsync<RegraNegocioException>(
            () => CriarHandler().Handle(command, CancellationToken.None));

        excecao.Regra.ShouldBe(RegrasPrescricao.MedicamentoInexistente);
        await _cenarios.UnitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task Handle_NaoEstruturadaComItens_LancaRegraEcf1001()
    {
        var command = new CriarPrescricaoCommand(
            Cenarios.PacienteId, TipoPrescricao.NaoEstruturada, Cenarios.NomeProfissional,
            Cenarios.RegistroProfissional, "Repouso.", [Cenarios.ItemDoCadastro]);

        var excecao = await Should.ThrowAsync<RegraNegocioException>(
            () => CriarHandler().Handle(command, CancellationToken.None));

        excecao.Regra.ShouldBe(RegrasPrescricao.SemItensNaNaoEstruturada);
    }

    [Fact]
    public async Task Handle_EstruturadaComTextoLivre_LancaRegraEcf1003()
    {
        var command = Estruturada(Cenarios.ItemDoCadastro) with { TextoLivre = "Texto que não cabe aqui." };

        var excecao = await Should.ThrowAsync<RegraNegocioException>(
            () => CriarHandler().Handle(command, CancellationToken.None));

        excecao.Regra.ShouldBe(RegrasPrescricao.SemTextoLivreNaEstruturada);
    }

    [Fact]
    public void Validator_PacienteNaoInformado_Invalido()
    {
        var command = Estruturada(Cenarios.ItemDoCadastro) with { PacienteId = 0 };

        var resultado = new CriarPrescricaoCommandValidator().Validate(command);

        resultado.IsValid.ShouldBeFalse();
        resultado.Errors.ShouldContain(erro => erro.PropertyName == nameof(CriarPrescricaoCommand.PacienteId));
    }

    [Fact]
    public void Validator_TipoDesconhecido_Invalido()
    {
        var command = Estruturada(Cenarios.ItemDoCadastro) with { Tipo = (TipoPrescricao)99 };

        var resultado = new CriarPrescricaoCommandValidator().Validate(command);

        resultado.Errors.ShouldContain(erro => erro.PropertyName == nameof(CriarPrescricaoCommand.Tipo));
    }

    private static CriarPrescricaoCommand Estruturada(params DadosItemPrescricao[] itens) => new(
        Cenarios.PacienteId, TipoPrescricao.Estruturada, Cenarios.NomeProfissional,
        Cenarios.RegistroProfissional, null, itens);

    private CriarPrescricaoCommandHandler CriarHandler() => new(
        _cenarios.Prescricoes, _cenarios.Pacientes, _cenarios.Medicamentos, _cenarios.UnitOfWork, _cenarios.Relogio);
}
