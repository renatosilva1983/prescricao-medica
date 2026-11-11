using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace PrescricaoMedica.Infrastructure.Persistence.Configurations;

/// <summary>
/// O MySQL não guarda o fuso: as datas são gravadas em UTC e, na leitura, marcadas como UTC
/// para a API devolver no formato ISO 8601 com "Z".
/// </summary>
internal static class UtcConverter
{
    public static readonly ValueConverter<DateTime, DateTime> Obrigatorio = new(
        valor => valor,
        valor => DateTime.SpecifyKind(valor, DateTimeKind.Utc));

    public static readonly ValueConverter<DateTime?, DateTime?> Opcional = new(
        valor => valor,
        valor => valor.HasValue ? DateTime.SpecifyKind(valor.Value, DateTimeKind.Utc) : valor);
}
