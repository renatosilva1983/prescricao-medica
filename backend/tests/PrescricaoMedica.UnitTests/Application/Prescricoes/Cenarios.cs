using NSubstitute;
using PrescricaoMedica.Application.Abstractions;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.UnitTests.Application.Prescricoes;

/// <summary>Dublês das portas e dados de referência dos testes dos casos de uso.</summary>
internal sealed class Cenarios
{
    public const int PacienteId = 1;
    public const int PrescricaoId = 10;
    public const string NomeProfissional = "Dra. Fernanda Moreira";
    public const string RegistroProfissional = "CRM-SP 123456";

    public static readonly DateTimeOffset Agora = new(2026, 11, 11, 12, 0, 0, TimeSpan.Zero);

    public static readonly DadosItemPrescricao ItemDoCadastro = new(
        MedicamentoId: 5,
        NomeMedicamentoLivre: null,
        FormaFarmaceutica: "Cápsula",
        DoseQuantidade: 500,
        DoseUnidade: "mg",
        Frequencia: "De 8 em 8 horas",
        ViaAdministracao: "Oral",
        DataInicio: new DateOnly(2026, 11, 11),
        DuracaoDias: 7,
        UsoContinuo: false,
        DataLimiteUso: null,
        Observacao: null);

    public Cenarios()
    {
        Pacientes.ExisteAsync(PacienteId, Arg.Any<CancellationToken>()).Returns(true);
        Medicamentos.ListarIdsInexistentesAsync(Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<int>>([]));
        Relogio.GetUtcNow().Returns(Agora);
    }

    public IPrescricaoRepository Prescricoes { get; } = Substitute.For<IPrescricaoRepository>();

    public IPacienteRepository Pacientes { get; } = Substitute.For<IPacienteRepository>();

    public IMedicamentoRepository Medicamentos { get; } = Substitute.For<IMedicamentoRepository>();

    public IUnitOfWork UnitOfWork { get; } = Substitute.For<IUnitOfWork>();

    public TimeProvider Relogio { get; } = Substitute.For<TimeProvider>();

    public void MedicamentosInexistentes(params int[] ids) =>
        Medicamentos.ListarIdsInexistentesAsync(Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyCollection<int>>(ids));

    public Prescricao PrescricaoExistente()
    {
        var prescricao = Prescricao.CriarNaoEstruturada(
            PacienteId, NomeProfissional, RegistroProfissional, "Repouso relativo.", Agora.UtcDateTime.AddDays(-1));
        Prescricoes.ObterPorIdAsync(PrescricaoId, Arg.Any<CancellationToken>()).Returns(prescricao);
        return prescricao;
    }
}
