using System.ComponentModel.DataAnnotations;

namespace PrescricaoMedica.Infrastructure.Persistence;

/// <summary>
/// Configuração do banco, na seção MySql. Em container, vem da variável de ambiente
/// MySql__ConnectionString, para a senha não ficar no repositório.
/// </summary>
public sealed class MySqlOptions
{
    public const string Secao = "MySql";

    [Required(ErrorMessage = "Configure MySql:ConnectionString (ou a variável MySql__ConnectionString).")]
    public string ConnectionString { get; init; } = string.Empty;
}
