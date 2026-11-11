using PrescricaoMedica.Application.Common.Results;

namespace PrescricaoMedica.Application.Prescricoes;

/// <summary>Erros esperados dos casos de uso de prescrição, num lugar só.</summary>
public static class PrescricaoErrors
{
    public static Error NaoEncontrada(int id) =>
        Error.NotFound("Prescricao.NaoEncontrada", $"Prescrição {id} não encontrada.");

    /// <summary>RG.1: o paciente precisa existir no cadastro.</summary>
    public static Error PacienteNaoEncontrado(int pacienteId) =>
        Error.NotFound("Paciente.NaoEncontrado", $"Paciente {pacienteId} não encontrado.");
}
