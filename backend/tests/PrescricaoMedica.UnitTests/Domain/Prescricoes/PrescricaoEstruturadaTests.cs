using PrescricaoMedica.Domain.Common;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Domain.Prescricoes;

/// <summary>ECF.10.03: receita estruturada e os dados de cada medicamento prescrito.</summary>
public class PrescricaoEstruturadaTests
{
    [Fact]
    public void CriarEstruturada_ItemDoCadastro_CriaComItensESemTexto()
    {
        var prescricao = Cenarios.Estruturada(Cenarios.ItemDoCadastro);

        prescricao.Tipo.ShouldBe(TipoPrescricao.Estruturada);
        prescricao.TextoLivre.ShouldBeNull();
        var item = prescricao.Itens.ShouldHaveSingleItem();
        item.MedicamentoId.ShouldBe(5);
        item.NomeMedicamentoLivre.ShouldBeNull();
        item.DoseQuantidade.ShouldBe(500m);
        item.DuracaoDias.ShouldBe(7);
    }

    [Fact]
    public void CriarEstruturada_ItemManipulado_CriaComNomeLivre()
    {
        var prescricao = Cenarios.Estruturada(Cenarios.ItemManipulado);

        var item = prescricao.Itens.ShouldHaveSingleItem();
        item.MedicamentoId.ShouldBeNull();
        item.NomeMedicamentoLivre.ShouldBe("Creme de ureia 10% (manipulado)");
    }

    [Fact]
    public void CriarEstruturada_SemItens_LancaRegraEcf1003()
    {
        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada());

        excecao.Regra.ShouldBe(RegrasPrescricao.QuantidadeItens);
        excecao.Regra.Requisito.ShouldBe("ECF.10.03");
    }

    [Fact]
    public void CriarEstruturada_ItensNoLimite_Cria()
    {
        var itens = Enumerable.Repeat(Cenarios.ItemDoCadastro, RegrasPrescricao.ItensMaximo).ToArray();

        var prescricao = Cenarios.Estruturada(itens);

        prescricao.Itens.Count.ShouldBe(RegrasPrescricao.ItensMaximo);
    }

    [Fact]
    public void CriarEstruturada_ItensAcimaDoLimite_LancaRegraEcf1003()
    {
        var itens = Enumerable.Repeat(Cenarios.ItemDoCadastro, RegrasPrescricao.ItensMaximo + 1).ToArray();

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(itens));

        excecao.Regra.ShouldBe(RegrasPrescricao.QuantidadeItens);
    }

    [Fact]
    public void CriarEstruturada_ItemComAsDuasOrigens_LancaRegraEcf1003()
    {
        var item = Cenarios.ItemDoCadastro with { NomeMedicamentoLivre = "Amoxicilina manipulada" };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.OrigemMedicamento);
    }

    [Fact]
    public void CriarEstruturada_ItemSemOrigem_LancaRegraEcf1003()
    {
        var item = Cenarios.ItemDoCadastro with { MedicamentoId = null, NomeMedicamentoLivre = "  " };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.OrigemMedicamento);
    }

    [Theory]
    [InlineData(nameof(DadosItemPrescricao.FormaFarmaceutica))]
    [InlineData(nameof(DadosItemPrescricao.DoseUnidade))]
    [InlineData(nameof(DadosItemPrescricao.Frequencia))]
    [InlineData(nameof(DadosItemPrescricao.ViaAdministracao))]
    [InlineData(nameof(DadosItemPrescricao.DataInicio))]
    public void CriarEstruturada_ItemSemCampoObrigatorio_LancaRegraEcf1003(string campo)
    {
        var item = SemCampo(campo);

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.DadosObrigatoriosItem);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CriarEstruturada_DoseNaoPositiva_LancaRegraEcf1003(int dose)
    {
        var item = Cenarios.ItemDoCadastro with { DoseQuantidade = dose };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.DosePositiva);
    }

    [Fact]
    public void CriarEstruturada_UsoContinuoSemDuracao_Cria()
    {
        var item = Cenarios.ItemDoCadastro with { UsoContinuo = true, DuracaoDias = null };

        var prescricao = Cenarios.Estruturada(item);

        prescricao.Itens.ShouldHaveSingleItem().UsoContinuo.ShouldBeTrue();
    }

    [Fact]
    public void CriarEstruturada_UsoContinuoComDuracao_LancaRegraEcf1003()
    {
        var item = Cenarios.ItemDoCadastro with { UsoContinuo = true, DuracaoDias = 30 };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.DuracaoTratamento);
    }

    [Theory]
    [InlineData(RegrasPrescricao.DuracaoMinimaDias)]
    [InlineData(RegrasPrescricao.DuracaoMaximaDias)]
    public void CriarEstruturada_DuracaoNosLimites_Cria(int duracao)
    {
        var item = Cenarios.ItemDoCadastro with { DuracaoDias = duracao };

        var prescricao = Cenarios.Estruturada(item);

        prescricao.Itens.ShouldHaveSingleItem().DuracaoDias.ShouldBe(duracao);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(366)]
    public void CriarEstruturada_DuracaoAusenteOuForaDaFaixa_LancaRegraEcf1003(int? duracao)
    {
        var item = Cenarios.ItemDoCadastro with { DuracaoDias = duracao };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.DuracaoTratamento);
    }

    [Fact]
    public void CriarEstruturada_PrazoIgualAoInicio_Cria()
    {
        var item = Cenarios.ItemDoCadastro with { DataLimiteUso = Cenarios.Inicio };

        var prescricao = Cenarios.Estruturada(item);

        prescricao.Itens.ShouldHaveSingleItem().DataLimiteUso.ShouldBe(Cenarios.Inicio);
    }

    [Fact]
    public void CriarEstruturada_PrazoAnteriorAoInicio_LancaRegraEcf1003()
    {
        var item = Cenarios.ItemDoCadastro with { DataLimiteUso = Cenarios.Inicio.AddDays(-1) };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.PrazoUtilizacao);
    }

    [Fact]
    public void CriarEstruturada_ObservacaoAcimaDoLimite_LancaRegraEcf1003()
    {
        var item = Cenarios.ItemDoCadastro with
        {
            Observacao = new string('a', RegrasPrescricao.ObservacaoTamanhoMaximo + 1),
        };

        var excecao = Should.Throw<RegraNegocioException>(() => Cenarios.Estruturada(item));

        excecao.Regra.ShouldBe(RegrasPrescricao.ObservacaoTamanho);
    }

    [Fact]
    public void CriarEstruturada_ObservacaoEmBranco_GuardaNulo()
    {
        var item = Cenarios.ItemDoCadastro with { Observacao = "   " };

        var prescricao = Cenarios.Estruturada(item);

        prescricao.Itens.ShouldHaveSingleItem().Observacao.ShouldBeNull();
    }

    [Fact]
    public void RevisarEstruturada_NovosItens_SubstituiItensEAtualizaData()
    {
        var prescricao = Cenarios.Estruturada(Cenarios.ItemDoCadastro);

        prescricao.RevisarEstruturada(
            Cenarios.NomeProfissional, Cenarios.RegistroProfissional,
            [Cenarios.ItemManipulado, Cenarios.ItemDoCadastro], Cenarios.Revisao);

        prescricao.Itens.Count.ShouldBe(2);
        prescricao.Itens.First().NomeMedicamentoLivre.ShouldBe(Cenarios.ItemManipulado.NomeMedicamentoLivre);
        prescricao.AtualizadaEmUtc.ShouldBe(Cenarios.Revisao);
    }

    [Fact]
    public void RevisarEstruturada_ItemInvalido_NaoAlteraPrescricao()
    {
        var prescricao = Cenarios.Estruturada(Cenarios.ItemDoCadastro);
        var itemInvalido = Cenarios.ItemDoCadastro with { DoseQuantidade = 0 };

        Should.Throw<RegraNegocioException>(() => prescricao.RevisarEstruturada(
            "Dr. Ricardo Tavares", "CRM-SP 654321", [Cenarios.ItemManipulado, itemInvalido], Cenarios.Revisao));

        prescricao.NomeProfissional.ShouldBe(Cenarios.NomeProfissional);
        prescricao.Itens.ShouldHaveSingleItem().MedicamentoId.ShouldBe(5);
        prescricao.AtualizadaEmUtc.ShouldBeNull();
    }

    [Fact]
    public void RevisarEstruturada_PrescricaoNaoEstruturada_TrocaTextoPorItens()
    {
        var prescricao = Cenarios.NaoEstruturada();

        prescricao.RevisarEstruturada(
            Cenarios.NomeProfissional, Cenarios.RegistroProfissional, [Cenarios.ItemDoCadastro], Cenarios.Revisao);

        prescricao.Tipo.ShouldBe(TipoPrescricao.Estruturada);
        prescricao.TextoLivre.ShouldBeNull();
        prescricao.Itens.ShouldHaveSingleItem();
    }

    private static DadosItemPrescricao SemCampo(string campo) => campo switch
    {
        nameof(DadosItemPrescricao.FormaFarmaceutica) => Cenarios.ItemDoCadastro with { FormaFarmaceutica = " " },
        nameof(DadosItemPrescricao.DoseUnidade) => Cenarios.ItemDoCadastro with { DoseUnidade = null },
        nameof(DadosItemPrescricao.Frequencia) => Cenarios.ItemDoCadastro with { Frequencia = "" },
        nameof(DadosItemPrescricao.ViaAdministracao) => Cenarios.ItemDoCadastro with { ViaAdministracao = null },
        nameof(DadosItemPrescricao.DataInicio) => Cenarios.ItemDoCadastro with { DataInicio = default },
        _ => throw new ArgumentOutOfRangeException(nameof(campo), campo, "Campo sem cenário de teste."),
    };
}
