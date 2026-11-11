namespace PrescricaoMedica.Domain.Common;

/// <summary>
/// Invariante do domínio violada. A API devolve 422 com o requisito e o código da regra.
/// </summary>
public sealed class RegraNegocioException(Regra regra) : Exception(regra.Mensagem)
{
    public Regra Regra { get; } = regra;
}
