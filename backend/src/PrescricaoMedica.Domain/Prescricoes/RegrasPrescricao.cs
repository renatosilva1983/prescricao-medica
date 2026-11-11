using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Domain.Prescricoes;

/// <summary>
/// Limites e regras da prescrição, com os mesmos códigos de docs/regras-de-negocio.md.
/// R01.3 e R03.2: a forma do agregado já impede o conteúdo do tipo errado (a não estruturada não
/// recebe itens e a estruturada não recebe texto livre); a Application usa as duas regras para
/// recusar, com 422, uma requisição que envie esse conteúdo.
/// </summary>
public static class RegrasPrescricao
{
    public const int TextoLivreTamanhoMaximo = 4000;
    public const int ItensMinimo = 1;
    public const int ItensMaximo = 30;
    public const int DuracaoMinimaDias = 1;
    public const int DuracaoMaximaDias = 365;
    public const int ObservacaoTamanhoMaximo = 500;

    private const string Ecf1001 = "ECF.10.01";
    private const string Ecf1003 = "ECF.10.03";
    private const string Geral = "Geral";

    public static readonly Regra TextoLivreObrigatorio = new(
        "R01.1", Ecf1001, "Informe o texto da receita não estruturada.");

    public static readonly Regra TextoLivreTamanho = new(
        "R01.2", Ecf1001, $"O texto da receita tem no máximo {TextoLivreTamanhoMaximo} caracteres.");

    public static readonly Regra SemItensNaNaoEstruturada = new(
        "R01.3", Ecf1001, "A receita não estruturada não tem itens; escreva a prescrição no texto livre.");

    public static readonly Regra QuantidadeItens = new(
        "R03.1", Ecf1003, $"A receita estruturada tem de {ItensMinimo} a {ItensMaximo} medicamentos.");

    public static readonly Regra SemTextoLivreNaEstruturada = new(
        "R03.2", Ecf1003, "A receita estruturada não tem texto livre; use os itens e a observação de cada item.");

    public static readonly Regra OrigemMedicamento = new(
        "R03.5", Ecf1003,
        "Informe um medicamento do cadastro ou o nome de um medicamento não cadastrado, e apenas um dos dois.");

    public static readonly Regra MedicamentoInexistente = new(
        "R03.6", Ecf1003, "O medicamento informado não existe no cadastro.");

    public static readonly Regra DadosObrigatoriosItem = new(
        "R03.7", Ecf1003,
        "Forma farmacêutica, dose, unidade da dose, frequência, via de administração e início do tratamento são obrigatórios.");

    public static readonly Regra DosePositiva = new(
        "R03.8", Ecf1003, "A dose deve ser maior que zero.");

    public static readonly Regra DuracaoTratamento = new(
        "R03.9", Ecf1003,
        $"Uso contínuo não tem duração; os demais têm duração de {DuracaoMinimaDias} a {DuracaoMaximaDias} dias.");

    public static readonly Regra PrazoUtilizacao = new(
        "R03.10", Ecf1003, "O prazo de utilização não pode ser anterior ao início do tratamento.");

    public static readonly Regra ObservacaoTamanho = new(
        "R03.11", Ecf1003, $"A observação tem no máximo {ObservacaoTamanhoMaximo} caracteres.");

    public static readonly Regra ProfissionalObrigatorio = new(
        "RG.2", Geral, "Informe o nome do profissional responsável e o registro no conselho.");
}
