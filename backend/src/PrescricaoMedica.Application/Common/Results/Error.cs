namespace PrescricaoMedica.Application.Common.Results;

/// <summary>Erro esperado de um caso de uso (Result Pattern): valor imutável, sem exceção.</summary>
public record Error(string Code, string Description, ErrorType Type)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
}
