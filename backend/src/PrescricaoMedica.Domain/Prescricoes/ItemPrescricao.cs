using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Domain.Prescricoes;

/// <summary>
/// Medicamento prescrito numa receita estruturada (ECF.10.03). Só é criado pelo agregado
/// <see cref="Prescricao"/>, que é quem garante as regras da receita como um todo.
/// </summary>
public sealed class ItemPrescricao : Entity
{
    private ItemPrescricao() { }

    public int PrescricaoId { get; private set; }

    /// <summary>Medicamento do cadastro (ECF.10.03 b). Nulo quando o nome é livre.</summary>
    public int? MedicamentoId { get; private set; }

    /// <summary>Medicamento fora do cadastro, como um manipulado (ECF.10.03 c).</summary>
    public string? NomeMedicamentoLivre { get; private set; }

    public string FormaFarmaceutica { get; private set; } = string.Empty;

    public decimal DoseQuantidade { get; private set; }

    public string DoseUnidade { get; private set; } = string.Empty;

    public string Frequencia { get; private set; } = string.Empty;

    public string ViaAdministracao { get; private set; } = string.Empty;

    public DateOnly DataInicio { get; private set; }

    /// <summary>Duração em dias; nula no uso contínuo.</summary>
    public int? DuracaoDias { get; private set; }

    public bool UsoContinuo { get; private set; }

    /// <summary>Prazo de utilização.</summary>
    public DateOnly? DataLimiteUso { get; private set; }

    public string? Observacao { get; private set; }

    internal static ItemPrescricao Criar(DadosItemPrescricao dados)
    {
        var nomeLivre = Texto.Normalizar(dados.NomeMedicamentoLivre);
        RegrasPrescricao.OrigemMedicamento.Garantir((dados.MedicamentoId is null) != (nomeLivre is null));

        var forma = Texto.Normalizar(dados.FormaFarmaceutica);
        var unidade = Texto.Normalizar(dados.DoseUnidade);
        var frequencia = Texto.Normalizar(dados.Frequencia);
        var via = Texto.Normalizar(dados.ViaAdministracao);
        RegrasPrescricao.DadosObrigatoriosItem.Garantir(
            forma is not null && unidade is not null && frequencia is not null && via is not null
            && dados.DataInicio != default);

        RegrasPrescricao.DosePositiva.Garantir(dados.DoseQuantidade > 0);

        RegrasPrescricao.DuracaoTratamento.Garantir(dados.UsoContinuo
            ? dados.DuracaoDias is null
            : dados.DuracaoDias is >= RegrasPrescricao.DuracaoMinimaDias and <= RegrasPrescricao.DuracaoMaximaDias);

        RegrasPrescricao.PrazoUtilizacao.Garantir(
            dados.DataLimiteUso is null || dados.DataLimiteUso >= dados.DataInicio);

        var observacao = Texto.Normalizar(dados.Observacao);
        RegrasPrescricao.ObservacaoTamanho.Garantir(
            observacao is null || observacao.Length <= RegrasPrescricao.ObservacaoTamanhoMaximo);

        return new ItemPrescricao
        {
            MedicamentoId = dados.MedicamentoId,
            NomeMedicamentoLivre = nomeLivre,
            FormaFarmaceutica = forma,
            DoseQuantidade = dados.DoseQuantidade,
            DoseUnidade = unidade,
            Frequencia = frequencia,
            ViaAdministracao = via,
            DataInicio = dados.DataInicio,
            DuracaoDias = dados.DuracaoDias,
            UsoContinuo = dados.UsoContinuo,
            DataLimiteUso = dados.DataLimiteUso,
            Observacao = observacao,
        };
    }
}
