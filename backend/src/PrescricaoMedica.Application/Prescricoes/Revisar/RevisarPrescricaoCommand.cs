using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes.Revisar;

/// <summary>Revisa o conteúdo de uma prescrição. O paciente não muda (RG.3).</summary>
public sealed record RevisarPrescricaoCommand(
    int Id,
    TipoPrescricao Tipo,
    string? NomeProfissional,
    string? RegistroProfissional,
    string? TextoLivre,
    IReadOnlyList<DadosItemPrescricao>? Itens) : ICommand, IConteudoPrescricao;
