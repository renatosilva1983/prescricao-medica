namespace PrescricaoMedica.Application.Common.Messaging;

/// <summary>Comando (escrita) sem valor de retorno.</summary>
public interface ICommand;

/// <summary>Comando (escrita) que devolve um valor, como o id criado.</summary>
public interface ICommand<TResponse>;
