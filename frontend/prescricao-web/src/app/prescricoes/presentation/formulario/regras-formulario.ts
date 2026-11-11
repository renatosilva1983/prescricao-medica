import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormControl,
  FormGroup,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';

import {
  CriarPrescricaoEntrada,
  ItemPrescricao,
  ItemPrescricaoEntrada,
  LIMITE_OBSERVACAO,
  LIMITE_TEXTO_LIVRE,
  MAX_DURACAO_DIAS,
  MAX_ITENS,
  MIN_DURACAO_DIAS,
  MIN_ITENS,
  Prescricao,
  TipoPrescricao,
} from '../../domain/prescricao';

export type ItemForm = FormGroup<{
  medicamentoId: FormControl<string | null>;
  nomeMedicamento: FormControl<string>;
  foraDoCadastro: FormControl<boolean>;
  nomeMedicamentoLivre: FormControl<string>;
  formaFarmaceutica: FormControl<string>;
  doseQuantidade: FormControl<number | null>;
  doseUnidade: FormControl<string>;
  frequencia: FormControl<string>;
  viaAdministracao: FormControl<string>;
  dataInicio: FormControl<string>;
  usoContinuo: FormControl<boolean>;
  duracaoDias: FormControl<number | null>;
  dataLimiteUso: FormControl<string>;
  observacao: FormControl<string>;
}>;

export type PrescricaoForm = FormGroup<{
  pacienteId: FormControl<string>;
  nomePaciente: FormControl<string>;
  nomeProfissional: FormControl<string>;
  registroProfissional: FormControl<string>;
  tipo: FormControl<TipoPrescricao>;
  textoLivre: FormControl<string>;
  itens: FormArray<ItemForm>;
}>;

/** Data local de hoje em yyyy-MM-dd. */
export function hojeIso(): string {
  const d = new Date();
  const p = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${p(d.getMonth() + 1)}-${p(d.getDate())}`;
}

/** Valida a quantidade de controles de um FormArray. */
export function quantidadeItens(min: number, max: number): ValidatorFn {
  return (controle: AbstractControl): ValidationErrors | null => {
    const n = (controle as FormArray).length;
    if (n < min) return { minItens: { min, atual: n } };
    if (n > max) return { maxItens: { max, atual: n } };
    return null;
  };
}

/** Regra: dataLimiteUso, se informada, não pode ser anterior a dataInicio. */
export const prazoPosteriorAoInicio: ValidatorFn = (grupo: AbstractControl): ValidationErrors | null => {
  const inicio = grupo.get('dataInicio')?.value as string | undefined;
  const limite = grupo.get('dataLimiteUso')?.value as string | undefined;
  return inicio && limite && limite < inicio ? { prazoAntesDoInicio: true } : null;
};

/** Regra: com uso contínuo a duração some (desabilitada e limpa); sem ele, é obrigatória (1–365). */
export function aplicarUsoContinuo(item: ItemForm, usoContinuo: boolean): void {
  const duracao = item.controls.duracaoDias;
  if (usoContinuo) {
    duracao.reset(null, { emitEvent: false });
    duracao.disable({ emitEvent: false });
  } else {
    duracao.enable({ emitEvent: false });
  }
  item.updateValueAndValidity({ emitEvent: false });
}

/** Regra: fora do cadastro exige nome livre; no cadastro exige medicamento selecionado. */
export function aplicarForaDoCadastro(item: ItemForm, foraDoCadastro: boolean): void {
  const { medicamentoId, nomeMedicamento, nomeMedicamentoLivre } = item.controls;
  if (foraDoCadastro) {
    medicamentoId.reset(null, { emitEvent: false });
    nomeMedicamento.reset('', { emitEvent: false });
    medicamentoId.disable({ emitEvent: false });
    nomeMedicamentoLivre.enable({ emitEvent: false });
  } else {
    nomeMedicamentoLivre.reset('', { emitEvent: false });
    nomeMedicamentoLivre.disable({ emitEvent: false });
    medicamentoId.enable({ emitEvent: false });
  }
  item.updateValueAndValidity({ emitEvent: false });
}

/** Regra: NaoEstruturada usa só o texto livre; Estruturada usa só os itens (1 a 30). */
export function aplicarTipo(form: PrescricaoForm, tipo: TipoPrescricao, fb: FormBuilder): void {
  const { textoLivre, itens } = form.controls;
  if (tipo === 'NaoEstruturada') {
    textoLivre.enable({ emitEvent: false });
    itens.disable({ emitEvent: false });
  } else {
    textoLivre.disable({ emitEvent: false });
    itens.enable({ emitEvent: false });
    if (itens.length === 0) {
      itens.push(criarItemForm(fb), { emitEvent: false });
    }
    // Reaplica as regras de cada item (o enable acima reabilita tudo).
    for (const item of itens.controls) {
      aplicarForaDoCadastro(item, item.controls.foraDoCadastro.value);
      aplicarUsoContinuo(item, item.controls.usoContinuo.value);
    }
  }
  form.updateValueAndValidity({ emitEvent: false });
}

export function criarItemForm(fb: FormBuilder, item?: ItemPrescricao): ItemForm {
  const n = fb.nonNullable;
  const foraDoCadastro = item ? !item.medicamentoCadastrado : false;
  const grupo: ItemForm = n.group(
    {
      medicamentoId: new FormControl<string | null>(foraDoCadastro ? null : (item?.medicamentoId ?? null), Validators.required),
      nomeMedicamento: n.control(foraDoCadastro ? '' : (item?.nomeMedicamento ?? '')),
      foraDoCadastro: n.control(foraDoCadastro),
      nomeMedicamentoLivre: n.control(foraDoCadastro ? (item?.nomeMedicamento ?? '') : '', [
        Validators.required,
        Validators.maxLength(200),
      ]),
      formaFarmaceutica: n.control(item?.formaFarmaceutica ?? '', Validators.required),
      doseQuantidade: new FormControl<number | null>(item?.doseQuantidade ?? null, [
        Validators.required,
        Validators.min(0.0001),
      ]),
      doseUnidade: n.control(item?.doseUnidade ?? '', Validators.required),
      frequencia: n.control(item?.frequencia ?? '', Validators.required),
      viaAdministracao: n.control(item?.viaAdministracao ?? '', Validators.required),
      dataInicio: n.control(item?.dataInicio?.slice(0, 10) ?? hojeIso(), Validators.required),
      usoContinuo: n.control(item?.usoContinuo ?? false),
      duracaoDias: new FormControl<number | null>(item?.duracaoDias ?? null, [
        Validators.required,
        Validators.min(MIN_DURACAO_DIAS),
        Validators.max(MAX_DURACAO_DIAS),
      ]),
      dataLimiteUso: n.control(item?.dataLimiteUso?.slice(0, 10) ?? ''),
      observacao: n.control(item?.observacao ?? '', Validators.maxLength(LIMITE_OBSERVACAO)),
    },
    { validators: prazoPosteriorAoInicio },
  );

  aplicarForaDoCadastro(grupo, grupo.controls.foraDoCadastro.value);
  aplicarUsoContinuo(grupo, grupo.controls.usoContinuo.value);
  grupo.controls.foraDoCadastro.valueChanges.subscribe((v) => aplicarForaDoCadastro(grupo, v));
  grupo.controls.usoContinuo.valueChanges.subscribe((v) => aplicarUsoContinuo(grupo, v));
  return grupo;
}

export function criarPrescricaoForm(fb: FormBuilder, prescricao?: Prescricao): PrescricaoForm {
  const n = fb.nonNullable;
  const form: PrescricaoForm = n.group({
    pacienteId: n.control(prescricao?.pacienteId ?? '', Validators.required),
    nomePaciente: n.control(prescricao?.nomePaciente ?? ''),
    nomeProfissional: n.control(prescricao?.nomeProfissional ?? '', [Validators.required, Validators.maxLength(200)]),
    registroProfissional: n.control(prescricao?.registroProfissional ?? '', [
      Validators.required,
      Validators.maxLength(50),
    ]),
    tipo: n.control<TipoPrescricao>(prescricao?.tipo ?? 'NaoEstruturada', Validators.required),
    textoLivre: n.control(prescricao?.textoLivre ?? '', [
      Validators.required,
      Validators.maxLength(LIMITE_TEXTO_LIVRE),
    ]),
    itens: fb.array<ItemForm>(
      (prescricao?.itens ?? []).map((i) => criarItemForm(fb, i)),
      quantidadeItens(MIN_ITENS, MAX_ITENS),
    ),
  });

  if (prescricao) {
    // Ao editar, o paciente não muda.
    form.controls.pacienteId.disable({ emitEvent: false });
  }

  aplicarTipo(form, form.controls.tipo.value, fb);
  form.controls.tipo.valueChanges.subscribe((t) => aplicarTipo(form, t, fb));
  return form;
}

function textoOpcional(valor: string | null | undefined): string | undefined {
  const t = valor?.trim();
  return t ? t : undefined;
}

export function paraItemEntrada(item: ItemForm): ItemPrescricaoEntrada {
  const v = item.getRawValue();
  return {
    medicamentoId: v.foraDoCadastro ? undefined : (v.medicamentoId ?? undefined),
    nomeMedicamentoLivre: v.foraDoCadastro ? textoOpcional(v.nomeMedicamentoLivre) : undefined,
    formaFarmaceutica: v.formaFarmaceutica.trim(),
    doseQuantidade: Number(v.doseQuantidade),
    doseUnidade: v.doseUnidade.trim(),
    frequencia: v.frequencia.trim(),
    viaAdministracao: v.viaAdministracao.trim(),
    dataInicio: v.dataInicio,
    duracaoDias: v.usoContinuo || v.duracaoDias == null ? undefined : Number(v.duracaoDias),
    usoContinuo: v.usoContinuo,
    dataLimiteUso: textoOpcional(v.dataLimiteUso),
    observacao: textoOpcional(v.observacao),
  };
}

/** Converte o formulário no corpo do POST (o PUT usa o mesmo corpo sem pacienteId). */
export function paraEntrada(form: PrescricaoForm): CriarPrescricaoEntrada {
  const v = form.getRawValue();
  const estruturada = v.tipo === 'Estruturada';
  return {
    pacienteId: v.pacienteId,
    tipo: v.tipo,
    nomeProfissional: v.nomeProfissional.trim(),
    registroProfissional: v.registroProfissional.trim(),
    textoLivre: estruturada ? undefined : v.textoLivre,
    itens: estruturada ? form.controls.itens.controls.map(paraItemEntrada) : undefined,
  };
}
