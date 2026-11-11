namespace PrescricaoMedica.Domain.Prescricoes;

public enum TipoPrescricao
{
    /// <summary>Receita em texto livre (ECF.10.01).</summary>
    NaoEstruturada = 1,

    /// <summary>Receita com itens estruturados (ECF.10.03).</summary>
    Estruturada = 2,
}
