using System.Diagnostics.CodeAnalysis;

namespace PrescricaoMedica.Domain.Common;

/// <summary>
/// Regra de negócio identificada como em docs/regras-de-negocio.md.
/// </summary>
/// <param name="Codigo">Identificador da regra derivada, por exemplo R03.5.</param>
/// <param name="Requisito">Requisito SBIS de origem, por exemplo ECF.10.03, ou Geral.</param>
/// <param name="Mensagem">Mensagem para o usuário quando a regra é violada.</param>
public sealed record Regra(string Codigo, string Requisito, string Mensagem)
{
    /// <summary>Lança <see cref="RegraNegocioException"/> se a condição não for atendida.</summary>
    public void Garantir([DoesNotReturnIf(false)] bool condicaoAtendida)
    {
        if (!condicaoAtendida)
        {
            throw new RegraNegocioException(this);
        }
    }
}
