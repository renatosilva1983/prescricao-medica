using FluentValidation;
using PrescricaoMedica.Domain.Prescricoes;

namespace PrescricaoMedica.Application.Prescricoes;

/// <summary>
/// Validação de formato (400), comum à emissão e à revisão: tipo conhecido e tamanho dos campos.
/// As regras de negócio (campos obrigatórios, dose, duração...) ficam no domínio (422).
/// </summary>
internal abstract class ConteudoPrescricaoValidator<T> : AbstractValidator<T>
    where T : IConteudoPrescricao
{
    protected ConteudoPrescricaoValidator()
    {
        RuleFor(conteudo => conteudo.Tipo)
            .IsInEnum()
            .WithMessage("Tipo de prescrição inválido. Use NaoEstruturada ou Estruturada.");
        RuleFor(conteudo => conteudo.NomeProfissional).MaximumLength(TamanhoCampo.NomeProfissional);
        RuleFor(conteudo => conteudo.RegistroProfissional).MaximumLength(TamanhoCampo.RegistroProfissional);
        RuleForEach(conteudo => conteudo.Itens!).SetValidator(new DadosItemPrescricaoValidator());
    }
}

internal sealed class DadosItemPrescricaoValidator : AbstractValidator<DadosItemPrescricao>
{
    public DadosItemPrescricaoValidator()
    {
        RuleFor(item => item.MedicamentoId).GreaterThan(0).When(item => item.MedicamentoId is not null);
        RuleFor(item => item.NomeMedicamentoLivre).MaximumLength(TamanhoCampo.NomeMedicamento);
        RuleFor(item => item.FormaFarmaceutica).MaximumLength(TamanhoCampo.FormaFarmaceutica);
        RuleFor(item => item.DoseUnidade).MaximumLength(TamanhoCampo.DoseUnidade);
        RuleFor(item => item.Frequencia).MaximumLength(TamanhoCampo.Frequencia);
        RuleFor(item => item.ViaAdministracao).MaximumLength(TamanhoCampo.ViaAdministracao);
    }
}

/// <summary>Tamanhos máximos, iguais aos das colunas em database/01-esquema.sql.</summary>
internal static class TamanhoCampo
{
    public const int NomeProfissional = 150;
    public const int RegistroProfissional = 30;
    public const int NomeMedicamento = 150;
    public const int FormaFarmaceutica = 60;
    public const int DoseUnidade = 20;
    public const int Frequencia = 60;
    public const int ViaAdministracao = 40;
}
