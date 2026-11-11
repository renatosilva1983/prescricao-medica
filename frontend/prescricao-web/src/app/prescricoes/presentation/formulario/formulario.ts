import { Component, OnInit, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';

import { Notificacoes } from '../../../core/notificacoes';
import { PrescricaoFacade } from '../../application/prescricao-facade';
import { ConsultasOData } from '../../data/consultas-odata';
import {
  LIMITE_OBSERVACAO,
  LIMITE_TEXTO_LIVRE,
  MAX_DURACAO_DIAS,
  MAX_ITENS,
  MIN_DURACAO_DIAS,
  Medicamento,
  Paciente,
  TIPOS_PRESCRICAO,
} from '../../domain/prescricao';
import { CampoBusca } from '../campo-busca/campo-busca';
import { ItemForm, PrescricaoForm, criarItemForm, criarPrescricaoForm, paraEntrada } from './regras-formulario';

@Component({
  selector: 'app-formulario',
  imports: [ReactiveFormsModule, RouterLink, CampoBusca],
  templateUrl: './formulario.html',
})
export class Formulario implements OnInit {
  /** Vem da rota /prescricoes/:id/editar (withComponentInputBinding). */
  readonly id = input<string>();

  protected readonly facade = inject(PrescricaoFacade);
  private readonly odata = inject(ConsultasOData);
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly notificacoes = inject(Notificacoes);

  protected readonly tipos = TIPOS_PRESCRICAO;
  protected readonly limiteTexto = LIMITE_TEXTO_LIVRE;
  protected readonly limiteObservacao = LIMITE_OBSERVACAO;
  protected readonly maxItens = MAX_ITENS;
  protected readonly minDuracao = MIN_DURACAO_DIAS;
  protected readonly maxDuracao = MAX_DURACAO_DIAS;

  protected readonly unidades = ['mg', 'g', 'mcg', 'mL', 'gota(s)', 'comprimido(s)', 'cápsula(s)', 'UI', 'aplicação(ões)'];
  protected readonly vias = ['Oral', 'Sublingual', 'Tópica', 'Intravenosa', 'Intramuscular', 'Subcutânea', 'Inalatória', 'Retal', 'Oftálmica', 'Nasal'];
  protected readonly formas = ['Comprimido', 'Cápsula', 'Solução oral', 'Suspensão oral', 'Xarope', 'Gotas', 'Pomada', 'Creme', 'Injetável', 'Manipulado'];

  protected form: PrescricaoForm = criarPrescricaoForm(this.fb);
  protected readonly pronto = signal(false);
  protected readonly enviado = signal(false);

  protected readonly buscarPacientes = (termo: string) => this.odata.buscarPacientes(termo);
  protected readonly rotuloPaciente = (p: Paciente) => p.nomeCompleto;
  protected readonly buscarMedicamentos = (termo: string) => this.odata.buscarMedicamentos(termo);
  protected readonly rotuloMedicamento = (m: Medicamento) =>
    [m.nome, m.concentracao, m.formaFarmaceutica].filter(Boolean).join(' · ');

  get editando(): boolean {
    return !!this.id();
  }

  get itens() {
    return this.form.controls.itens;
  }

  async ngOnInit(): Promise<void> {
    const id = this.id();
    if (id) {
      const prescricao = await this.facade.obter(id);
      if (!prescricao) {
        return;
      }
      this.form = criarPrescricaoForm(this.fb, prescricao);
    }
    this.pronto.set(true);
  }

  protected selecionarPaciente(p: Paciente): void {
    this.form.patchValue({ pacienteId: p.id, nomePaciente: p.nomeCompleto });
  }

  protected limparPaciente(): void {
    this.form.patchValue({ pacienteId: '', nomePaciente: '' });
  }

  protected selecionarMedicamento(item: ItemForm, m: Medicamento): void {
    item.patchValue({
      medicamentoId: m.id,
      nomeMedicamento: m.nome,
      formaFarmaceutica: item.controls.formaFarmaceutica.value || m.formaFarmaceutica,
    });
  }

  protected limparMedicamento(item: ItemForm): void {
    item.patchValue({ medicamentoId: null, nomeMedicamento: '' });
  }

  protected adicionarItem(): void {
    if (this.itens.length < MAX_ITENS) {
      this.itens.push(criarItemForm(this.fb));
    }
  }

  protected removerItem(indice: number): void {
    if (this.itens.length > 1) {
      this.itens.removeAt(indice);
    }
  }

  /** Mostra erro de um controle depois de tocado ou de tentar enviar. */
  protected erro(controle: { invalid: boolean; touched: boolean }): boolean {
    return controle.invalid && (controle.touched || this.enviado());
  }

  protected async salvar(): Promise<void> {
    this.enviado.set(true);
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      this.notificacoes.erro('Revise os campos destacados antes de salvar.');
      return;
    }
    const id = await this.facade.salvar(paraEntrada(this.form), this.id());
    if (id) {
      this.notificacoes.sucesso(this.editando ? 'Prescrição atualizada.' : 'Prescrição criada.');
      await this.router.navigate(['/prescricoes', id]);
    }
  }
}
