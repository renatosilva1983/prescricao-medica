using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Api.Contracts;

/// <summary>Corpo do POST /api/prescricoes.</summary>
public sealed record CriarPrescricaoRequest(
    int PacienteId,
    TipoPrescricao Tipo,
    string? NomeProfissional,
    string? RegistroProfissional,
    string? TextoLivre,
    IReadOnlyList<DadosItemPrescricao>? Itens);

/// <summary>Corpo do PUT /api/prescricoes/{id}. Não tem paciente: ele não muda na revisão (RG.3).</summary>
public sealed record RevisarPrescricaoRequest(
    TipoPrescricao Tipo,
    string? NomeProfissional,
    string? RegistroProfissional,
    string? TextoLivre,
    IReadOnlyList<DadosItemPrescricao>? Itens);
