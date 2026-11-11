using PrescricaoMedica.Domain.Common;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Domain.Prescricoes;

/// <summary>ECF.10.01: receita não estruturada, e as regras gerais da prescrição.</summary>
public class PrescricaoNaoEstruturadaTests
{
    [Fact]
    public void CriarNaoEstruturada_DadosValidos_CriaComTextoESemItens()
    {
        var prescricao = Prescricao.CriarNaoEstruturada(
            Cenarios.PacienteId, "  Dra. Fernanda Moreira ", "CRM-SP 123456", "  Repouso relativo.  ", Cenarios.Emissao);

        prescricao.Tipo.ShouldBe(TipoPrescricao.NaoEstruturada);
        prescricao.PacienteId.ShouldBe(Cenarios.PacienteId);
        prescricao.NomeProfissional.ShouldBe("Dra. Fernanda Moreira");
        prescricao.TextoLivre.ShouldBe("Repouso relativo.");
        prescricao.Itens.ShouldBeEmpty();
        prescricao.EmitidaEmUtc.ShouldBe(Cenarios.Emissao);
        prescricao.AtualizadaEmUtc.ShouldBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void CriarNaoEstruturada_SemTexto_LancaRegraEcf1001(string? texto)
    {
        var excecao = Should.Throw<RegraNegocioException>(() => Prescricao.CriarNaoEstruturada(
            Cenarios.PacienteId, Cenarios.NomeProfissional, Cenarios.RegistroProfissional, texto, Cenarios.Emissao));

        excecao.Regra.ShouldBe(RegrasPrescricao.TextoLivreObrigatorio);
        excecao.Regra.Requisito.ShouldBe("ECF.10.01");
    }

    [Fact]
    public void CriarNaoEstruturada_TextoNoLimite_Cria()
    {
        var texto = new string('a', RegrasPrescricao.TextoLivreTamanhoMaximo);

        var prescricao = Prescricao.CriarNaoEstruturada(
            Cenarios.PacienteId, Cenarios.NomeProfissional, Cenarios.RegistroProfissional, texto, Cenarios.Emissao);

        prescricao.TextoLivre!.Length.ShouldBe(RegrasPrescricao.TextoLivreTamanhoMaximo);
    }

    [Fact]
    public void CriarNaoEstruturada_TextoAcimaDoLimite_LancaRegraEcf1001()
    {
        var texto = new string('a', RegrasPrescricao.TextoLivreTamanhoMaximo + 1);

        var excecao = Should.Throw<RegraNegocioException>(() => Prescricao.CriarNaoEstruturada(
            Cenarios.PacienteId, Cenarios.NomeProfissional, Cenarios.RegistroProfissional, texto, Cenarios.Emissao));

        excecao.Regra.ShouldBe(RegrasPrescricao.TextoLivreTamanho);
    }

    [Theory]
    [InlineData(null, "CRM-SP 123456")]
    [InlineData("  ", "CRM-SP 123456")]
    [InlineData("Dra. Fernanda Moreira", null)]
    [InlineData("Dra. Fernanda Moreira", "")]
    public void CriarNaoEstruturada_SemProfissional_LancaRegraGeral(string? nome, string? registro)
    {
        var excecao = Should.Throw<RegraNegocioException>(() => Prescricao.CriarNaoEstruturada(
            Cenarios.PacienteId, nome, registro, Cenarios.TextoLivre, Cenarios.Emissao));

        excecao.Regra.ShouldBe(RegrasPrescricao.ProfissionalObrigatorio);
    }

    [Fact]
    public void RevisarNaoEstruturada_DadosValidos_AtualizaTextoEDataSemMudarPacienteNemEmissao()
    {
        var prescricao = Cenarios.NaoEstruturada();

        prescricao.RevisarNaoEstruturada("Dr. Ricardo Tavares", "CRM-SP 654321", "Texto revisado.", Cenarios.Revisao);

        prescricao.TextoLivre.ShouldBe("Texto revisado.");
        prescricao.NomeProfissional.ShouldBe("Dr. Ricardo Tavares");
        prescricao.PacienteId.ShouldBe(Cenarios.PacienteId);
        prescricao.EmitidaEmUtc.ShouldBe(Cenarios.Emissao);
        prescricao.AtualizadaEmUtc.ShouldBe(Cenarios.Revisao);
    }

    [Fact]
    public void RevisarNaoEstruturada_TextoInvalido_NaoAlteraPrescricao()
    {
        var prescricao = Cenarios.NaoEstruturada();

        Should.Throw<RegraNegocioException>(() => prescricao.RevisarNaoEstruturada(
            "Dr. Ricardo Tavares", "CRM-SP 654321", " ", Cenarios.Revisao));

        prescricao.TextoLivre.ShouldBe(Cenarios.TextoLivre);
        prescricao.NomeProfissional.ShouldBe(Cenarios.NomeProfissional);
        prescricao.AtualizadaEmUtc.ShouldBeNull();
    }

    [Fact]
    public void RevisarNaoEstruturada_PrescricaoEstruturada_TrocaItensPorTexto()
    {
        var prescricao = Cenarios.Estruturada(Cenarios.ItemDoCadastro);

        prescricao.RevisarNaoEstruturada(
            Cenarios.NomeProfissional, Cenarios.RegistroProfissional, Cenarios.TextoLivre, Cenarios.Revisao);

        prescricao.Tipo.ShouldBe(TipoPrescricao.NaoEstruturada);
        prescricao.TextoLivre.ShouldBe(Cenarios.TextoLivre);
        prescricao.Itens.ShouldBeEmpty();
    }
}
