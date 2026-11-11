using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Domain.Prescricoes;

/// <summary>Dados válidos de referência; cada teste altera só o que quer violar.</summary>
internal static class Cenarios
{
    public const int PacienteId = 1;
    public const string NomeProfissional = "Dra. Fernanda Moreira";
    public const string RegistroProfissional = "CRM-SP 123456";
    public const string TextoLivre = "Repouso relativo por 3 dias.";

    public static readonly DateTime Emissao = new(2026, 11, 11, 12, 0, 0, DateTimeKind.Utc);
    public static readonly DateTime Revisao = new(2026, 11, 12, 9, 30, 0, DateTimeKind.Utc);
    public static readonly DateOnly Inicio = new(2026, 11, 11);

    public static readonly DadosItemPrescricao ItemDoCadastro = new(
        MedicamentoId: 5,
        NomeMedicamentoLivre: null,
        FormaFarmaceutica: "Cápsula",
        DoseQuantidade: 500,
        DoseUnidade: "mg",
        Frequencia: "De 8 em 8 horas",
        ViaAdministracao: "Oral",
        DataInicio: Inicio,
        DuracaoDias: 7,
        UsoContinuo: false,
        DataLimiteUso: null,
        Observacao: "Tomar após as refeições.");

    public static readonly DadosItemPrescricao ItemManipulado = ItemDoCadastro with
    {
        MedicamentoId = null,
        NomeMedicamentoLivre = "Creme de ureia 10% (manipulado)",
        FormaFarmaceutica = "Creme",
    };

    public static Prescricao NaoEstruturada() => Prescricao.CriarNaoEstruturada(
        PacienteId, NomeProfissional, RegistroProfissional, TextoLivre, Emissao);

    public static Prescricao Estruturada(params DadosItemPrescricao[] itens) => Prescricao.CriarEstruturada(
        PacienteId, NomeProfissional, RegistroProfissional, itens, Emissao);
}
