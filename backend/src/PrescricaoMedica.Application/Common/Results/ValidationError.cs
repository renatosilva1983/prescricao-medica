namespace PrescricaoMedica.Application.Common.Results;

/// <summary>Erros de formato da entrada, agrupados por campo. Produzido pelo decorator de validação.</summary>
public sealed record ValidationError(IReadOnlyDictionary<string, string[]> Errors)
    : Error("Validacao", "Um ou mais campos são inválidos.", ErrorType.Validation);
