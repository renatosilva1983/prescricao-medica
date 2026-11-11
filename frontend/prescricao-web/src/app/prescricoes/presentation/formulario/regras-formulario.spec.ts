import { TestBed } from '@angular/core/testing';
import { FormBuilder } from '@angular/forms';

import { ItemForm, criarItemForm, criarPrescricaoForm, paraEntrada } from './regras-formulario';

function preencherItem(item: ItemForm): void {
  item.patchValue({
    medicamentoId: 'm1',
    nomeMedicamento: 'Amoxicilina',
    formaFarmaceutica: 'Cápsula',
    doseQuantidade: 500,
    doseUnidade: 'mg',
    frequencia: '8 em 8 horas',
    viaAdministracao: 'Oral',
    dataInicio: '2026-11-11',
    duracaoDias: 7,
  });
}

describe('regras do formulário de prescrição', () => {
  let fb: FormBuilder;

  beforeEach(() => {
    fb = TestBed.inject(FormBuilder);
  });

  describe('uso contínuo', () => {
    it('ao marcar, desabilita e limpa a duração, e o item fica válido sem ela', () => {
      const item = criarItemForm(fb);
      preencherItem(item);
      expect(item.valid).toBe(true);

      item.controls.usoContinuo.setValue(true);

      expect(item.controls.duracaoDias.disabled).toBe(true);
      expect(item.controls.duracaoDias.value).toBeNull();
      expect(item.valid).toBe(true);
    });

    it('ao desmarcar, a duração volta a ser obrigatória e limitada a 1–365', () => {
      const item = criarItemForm(fb);
      preencherItem(item);
      item.controls.usoContinuo.setValue(true);
      item.controls.usoContinuo.setValue(false);

      expect(item.controls.duracaoDias.enabled).toBe(true);
      expect(item.controls.duracaoDias.hasError('required')).toBe(true);

      item.controls.duracaoDias.setValue(366);
      expect(item.controls.duracaoDias.hasError('max')).toBe(true);
      item.controls.duracaoDias.setValue(0);
      expect(item.controls.duracaoDias.hasError('min')).toBe(true);
      item.controls.duracaoDias.setValue(30);
      expect(item.valid).toBe(true);
    });
  });

  it('fora do cadastro exige nome livre e dispensa o medicamento do cadastro', () => {
    const item = criarItemForm(fb);
    preencherItem(item);

    item.controls.foraDoCadastro.setValue(true);
    expect(item.controls.medicamentoId.disabled).toBe(true);
    expect(item.controls.nomeMedicamentoLivre.hasError('required')).toBe(true);
    expect(item.valid).toBe(false);

    item.controls.nomeMedicamentoLivre.setValue('Fórmula manipulada X');
    expect(item.valid).toBe(true);
  });

  it('prazo de utilização não pode ser anterior ao início', () => {
    const item = criarItemForm(fb);
    preencherItem(item);
    item.controls.dataLimiteUso.setValue('2026-11-01');
    expect(item.hasError('prazoAntesDoInicio')).toBe(true);
  });

  it('tipo Estruturada ignora o texto livre e exige de 1 a 30 itens', () => {
    const form = criarPrescricaoForm(fb);
    form.patchValue({ pacienteId: 'p1', nomeProfissional: 'Dra. Ana', registroProfissional: 'CRM-SP 123456' });

    form.controls.tipo.setValue('Estruturada');

    expect(form.controls.textoLivre.disabled).toBe(true);
    expect(form.controls.itens.length).toBe(1);
    preencherItem(form.controls.itens.at(0));
    expect(form.valid).toBe(true);

    form.controls.itens.removeAt(0);
    expect(form.controls.itens.hasError('minItens')).toBe(true);
  });

  it('gera o corpo da API sem duração para uso contínuo e com nome livre fora do cadastro', () => {
    const form = criarPrescricaoForm(fb);
    form.patchValue({ pacienteId: 'p1', nomeProfissional: 'Dra. Ana', registroProfissional: 'CRM-SP 123456' });
    form.controls.tipo.setValue('Estruturada');
    const item = form.controls.itens.at(0);
    preencherItem(item);
    item.controls.usoContinuo.setValue(true);
    item.controls.foraDoCadastro.setValue(true);
    item.controls.nomeMedicamentoLivre.setValue('Manipulado');

    const corpo = paraEntrada(form);

    expect(corpo.textoLivre).toBeUndefined();
    expect(corpo.itens).toHaveLength(1);
    expect(corpo.itens![0]).toMatchObject({
      nomeMedicamentoLivre: 'Manipulado',
      usoContinuo: true,
      dataInicio: '2026-11-11',
    });
    expect(corpo.itens![0].medicamentoId).toBeUndefined();
    expect(corpo.itens![0].duracaoDias).toBeUndefined();
  });
});
