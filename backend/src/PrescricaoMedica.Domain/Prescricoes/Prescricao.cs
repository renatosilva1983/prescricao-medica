using PrescricaoMedica.Domain.Common;

namespace PrescricaoMedica.Domain.Prescricoes;

/// <summary>
/// Agregado da prescrição médica. Só existe em estado válido: cada operação valida tudo
/// antes de alterar qualquer dado, então uma violação não deixa a prescrição pela metade.
/// </summary>
public sealed class Prescricao : Entity
{
    private readonly List<ItemPrescricao> _itens = [];

    private Prescricao() { }

    /// <summary>Não muda numa revisão (RG.3).</summary>
    public int PacienteId { get; private set; }

    public TipoPrescricao Tipo { get; private set; }

    public string NomeProfissional { get; private set; } = string.Empty;

    /// <summary>Conselho, número e UF. Ex.: CRM-SP 123456.</summary>
    public string RegistroProfissional { get; private set; } = string.Empty;

    /// <summary>Conteúdo da receita não estruturada (ECF.10.01); nulo na estruturada.</summary>
    public string? TextoLivre { get; private set; }

    /// <summary>Definida pelo sistema na emissão (RG.4).</summary>
    public DateTime EmitidaEmUtc { get; private set; }

    /// <summary>Data da última revisão (RG.4).</summary>
    public DateTime? AtualizadaEmUtc { get; private set; }

    /// <summary>Itens da receita estruturada (ECF.10.03); vazio na não estruturada.</summary>
    public IReadOnlyCollection<ItemPrescricao> Itens => _itens.AsReadOnly();

    /// <summary>Emite uma receita não estruturada, em texto livre (ECF.10.01).</summary>
    public static Prescricao CriarNaoEstruturada(
        int pacienteId, string? nomeProfissional, string? registroProfissional, string? textoLivre, DateTime agoraUtc)
    {
        var (nome, registro) = ValidarProfissional(nomeProfissional, registroProfissional);
        var texto = ValidarTextoLivre(textoLivre);

        var prescricao = new Prescricao { PacienteId = pacienteId, EmitidaEmUtc = agoraUtc };
        prescricao.Aplicar(TipoPrescricao.NaoEstruturada, nome, registro, texto, []);
        return prescricao;
    }

    /// <summary>Emite uma receita estruturada, com a lista de medicamentos (ECF.10.03).</summary>
    public static Prescricao CriarEstruturada(
        int pacienteId, string? nomeProfissional, string? registroProfissional,
        IReadOnlyCollection<DadosItemPrescricao> itens, DateTime agoraUtc)
    {
        var (nome, registro) = ValidarProfissional(nomeProfissional, registroProfissional);
        var novosItens = CriarItens(itens);

        var prescricao = new Prescricao { PacienteId = pacienteId, EmitidaEmUtc = agoraUtc };
        prescricao.Aplicar(TipoPrescricao.Estruturada, nome, registro, null, novosItens);
        return prescricao;
    }

    /// <summary>Revisa a prescrição como não estruturada; itens anteriores são removidos.</summary>
    public void RevisarNaoEstruturada(
        string? nomeProfissional, string? registroProfissional, string? textoLivre, DateTime agoraUtc)
    {
        var (nome, registro) = ValidarProfissional(nomeProfissional, registroProfissional);
        var texto = ValidarTextoLivre(textoLivre);

        Aplicar(TipoPrescricao.NaoEstruturada, nome, registro, texto, []);
        AtualizadaEmUtc = agoraUtc;
    }

    /// <summary>Revisa a prescrição como estruturada; os itens são substituídos pelos informados.</summary>
    public void RevisarEstruturada(
        string? nomeProfissional, string? registroProfissional,
        IReadOnlyCollection<DadosItemPrescricao> itens, DateTime agoraUtc)
    {
        var (nome, registro) = ValidarProfissional(nomeProfissional, registroProfissional);
        var novosItens = CriarItens(itens);

        Aplicar(TipoPrescricao.Estruturada, nome, registro, null, novosItens);
        AtualizadaEmUtc = agoraUtc;
    }

    private void Aplicar(
        TipoPrescricao tipo, string nome, string registro, string? texto, IEnumerable<ItemPrescricao> itens)
    {
        Tipo = tipo;
        NomeProfissional = nome;
        RegistroProfissional = registro;
        TextoLivre = texto;
        _itens.Clear();
        _itens.AddRange(itens);
    }

    private static (string Nome, string Registro) ValidarProfissional(string? nome, string? registro)
    {
        var nomeNormalizado = Texto.Normalizar(nome);
        var registroNormalizado = Texto.Normalizar(registro);
        RegrasPrescricao.ProfissionalObrigatorio.Garantir(nomeNormalizado is not null && registroNormalizado is not null);
        return (nomeNormalizado, registroNormalizado);
    }

    private static string ValidarTextoLivre(string? textoLivre)
    {
        var texto = Texto.Normalizar(textoLivre);
        RegrasPrescricao.TextoLivreObrigatorio.Garantir(texto is not null);
        RegrasPrescricao.TextoLivreTamanho.Garantir(texto.Length <= RegrasPrescricao.TextoLivreTamanhoMaximo);
        return texto;
    }

    private static List<ItemPrescricao> CriarItens(IReadOnlyCollection<DadosItemPrescricao> itens)
    {
        RegrasPrescricao.QuantidadeItens.Garantir(
            itens.Count is >= RegrasPrescricao.ItensMinimo and <= RegrasPrescricao.ItensMaximo);
        return itens.Select(ItemPrescricao.Criar).ToList();
    }
}
