import { Injectable, computed, inject, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';

import { extrairMensagemErro } from '../../core/problem-details';
import { ConsultasOData } from '../data/consultas-odata';
import { PrescricoesApi } from '../data/prescricoes-api';
import {
  CriarPrescricaoEntrada,
  FiltroPrescricoes,
  Prescricao,
  PrescricaoResumo,
} from '../domain/prescricao';

export const FILTRO_INICIAL: FiltroPrescricoes = {
  nomePaciente: '',
  tipo: '',
  ordem: 'desc',
  pagina: 1,
  tamanhoPagina: 10,
};

/** Casos de uso de prescrição expostos às telas, com estado em signals. */
@Injectable({ providedIn: 'root' })
export class PrescricaoFacade {
  private readonly api = inject(PrescricoesApi);
  private readonly odata = inject(ConsultasOData);

  readonly lista = signal<PrescricaoResumo[]>([]);
  readonly total = signal(0);
  readonly filtro = signal<FiltroPrescricoes>(FILTRO_INICIAL);
  readonly carregando = signal(false);
  readonly salvando = signal(false);
  readonly erro = signal<string | null>(null);

  readonly totalPaginas = computed(() =>
    Math.max(1, Math.ceil(this.total() / this.filtro().tamanhoPagina)),
  );

  /** Recarrega a lista aplicando (opcionalmente) alterações ao filtro atual. */
  async carregar(alteracoes: Partial<FiltroPrescricoes> = {}): Promise<void> {
    this.filtro.update((f) => ({ ...f, ...alteracoes }));
    this.carregando.set(true);
    this.erro.set(null);
    try {
      const pagina = await firstValueFrom(this.odata.listarPrescricoes(this.filtro()));
      this.lista.set(pagina.itens);
      this.total.set(pagina.total);
    } catch (e) {
      this.erro.set(extrairMensagemErro(e));
      this.lista.set([]);
      this.total.set(0);
    } finally {
      this.carregando.set(false);
    }
  }

  async obter(id: string): Promise<Prescricao | null> {
    this.carregando.set(true);
    this.erro.set(null);
    try {
      return await firstValueFrom(this.api.obter(id));
    } catch (e) {
      this.erro.set(extrairMensagemErro(e));
      return null;
    } finally {
      this.carregando.set(false);
    }
  }

  /**
   * Cria (sem `id`) ou atualiza (com `id`) uma prescrição.
   * Retorna o id salvo, ou `null` em caso de erro (mensagem em `erro`).
   */
  async salvar(entrada: CriarPrescricaoEntrada, id?: string): Promise<string | null> {
    this.salvando.set(true);
    this.erro.set(null);
    try {
      if (id) {
        const { pacienteId: _ignorado, ...semPaciente } = entrada;
        await firstValueFrom(this.api.atualizar(id, semPaciente));
        return id;
      }
      return await firstValueFrom(this.api.criar(entrada));
    } catch (e) {
      this.erro.set(extrairMensagemErro(e));
      return null;
    } finally {
      this.salvando.set(false);
    }
  }

  /** Exclui e recarrega a página atual (volta uma página se ela ficar vazia). */
  async excluir(id: string): Promise<boolean> {
    this.erro.set(null);
    try {
      await firstValueFrom(this.api.excluir(id));
    } catch (e) {
      this.erro.set(extrairMensagemErro(e));
      return false;
    }
    const f = this.filtro();
    const voltarPagina = this.lista().length === 1 && f.pagina > 1;
    await this.carregar(voltarPagina ? { pagina: f.pagina - 1 } : {});
    return true;
  }
}
