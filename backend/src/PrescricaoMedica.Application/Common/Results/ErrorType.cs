namespace PrescricaoMedica.Application.Common.Results;

/// <summary>Categoria do erro. A API usa o tipo para escolher o status HTTP.</summary>
public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
}
