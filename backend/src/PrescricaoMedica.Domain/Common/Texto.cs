namespace PrescricaoMedica.Domain.Common;

internal static class Texto
{
    /// <summary>Remove espaços das pontas; texto vazio ou só com espaços vira nulo.</summary>
    public static string? Normalizar(string? valor) =>
        string.IsNullOrWhiteSpace(valor) ? null : valor.Trim();
}
