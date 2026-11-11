using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes;

/// <summary>Passos comuns à emissão e à revisão, para os dois handlers não repetirem código.</summary>
internal static class ConteudoPrescricaoExtensions
{
    public static IReadOnlyList<DadosItemPrescricao> ItensOuVazio(this IConteudoPrescricao conteudo) =>
        conteudo.Itens ?? [];

    /// <summary>R01.3 e R03.2: cada tipo de receita recebe só o seu conteúdo.</summary>
    public static void GarantirConteudoDoTipo(this IConteudoPrescricao conteudo)
    {
        if (conteudo.Tipo == TipoPrescricao.NaoEstruturada)
        {
            RegrasPrescricao.SemItensNaNaoEstruturada.Garantir(conteudo.ItensOuVazio().Count == 0);
        }
        else
        {
            RegrasPrescricao.SemTextoLivreNaEstruturada.Garantir(string.IsNullOrWhiteSpace(conteudo.TextoLivre));
        }
    }

    /// <summary>R03.6: um medicamento escolhido do cadastro precisa existir.</summary>
    public static async Task GarantirMedicamentosCadastradosAsync(
        this IConteudoPrescricao conteudo, IMedicamentoRepository medicamentos, CancellationToken cancellationToken)
    {
        var ids = conteudo.ItensOuVazio()
            .Where(item => item.MedicamentoId is not null)
            .Select(item => item.MedicamentoId!.Value)
            .Distinct()
            .ToList();

        if (ids.Count == 0)
        {
            return;
        }

        var inexistentes = await medicamentos.ListarIdsInexistentesAsync(ids, cancellationToken);
        RegrasPrescricao.MedicamentoInexistente.Garantir(inexistentes.Count == 0);
    }

    public static Prescricao Emitir(this IConteudoPrescricao conteudo, int pacienteId, DateTime agoraUtc) =>
        conteudo.Tipo == TipoPrescricao.NaoEstruturada
            ? Prescricao.CriarNaoEstruturada(
                pacienteId, conteudo.NomeProfissional, conteudo.RegistroProfissional, conteudo.TextoLivre, agoraUtc)
            : Prescricao.CriarEstruturada(
                pacienteId, conteudo.NomeProfissional, conteudo.RegistroProfissional, conteudo.ItensOuVazio(), agoraUtc);

    public static void Aplicar(this IConteudoPrescricao conteudo, Prescricao prescricao, DateTime agoraUtc)
    {
        if (conteudo.Tipo == TipoPrescricao.NaoEstruturada)
        {
            prescricao.RevisarNaoEstruturada(
                conteudo.NomeProfissional, conteudo.RegistroProfissional, conteudo.TextoLivre, agoraUtc);
        }
        else
        {
            prescricao.RevisarEstruturada(
                conteudo.NomeProfissional, conteudo.RegistroProfissional, conteudo.ItensOuVazio(), agoraUtc);
        }
    }
}
