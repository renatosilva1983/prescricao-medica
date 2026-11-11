using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes;

/// <summary>
/// Conteúdo de uma prescrição, igual na emissão e na revisão. O tipo decide o que vale:
/// NaoEstruturada usa o texto livre (ECF.10.01); Estruturada usa os itens (ECF.10.03).
/// </summary>
public interface IConteudoPrescricao
{
    TipoPrescricao Tipo { get; }

    string? NomeProfissional { get; }

    string? RegistroProfissional { get; }

    string? TextoLivre { get; }

    IReadOnlyList<DadosItemPrescricao>? Itens { get; }
}
