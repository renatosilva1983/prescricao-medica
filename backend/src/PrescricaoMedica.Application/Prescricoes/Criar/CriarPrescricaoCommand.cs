using PrescricaoMedica.Application.Common.Messaging;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes.Criar;

/// <summary>Emite uma prescrição; devolve o id criado.</summary>
public sealed record CriarPrescricaoCommand(
    int PacienteId,
    TipoPrescricao Tipo,
    string? NomeProfissional,
    string? RegistroProfissional,
    string? TextoLivre,
    IReadOnlyList<DadosItemPrescricao>? Itens) : ICommand<int>, IConteudoPrescricao;
