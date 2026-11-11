import { DatePipe } from '@angular/common';
import { Component, DestroyRef, OnInit, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { Subject, debounceTime, distinctUntilChanged } from 'rxjs';

import { Notificacoes } from '../../../core/notificacoes';
import { PrescricaoFacade } from '../../application/prescricao-facade';
import { PrescricaoResumo, TIPOS_PRESCRICAO, TipoPrescricao } from '../../domain/prescricao';

@Component({
  selector: 'app-lista',
  imports: [RouterLink, DatePipe],
  templateUrl: './lista.html',
})
export class Lista implements OnInit {
  protected readonly facade = inject(PrescricaoFacade);
  private readonly notificacoes = inject(Notificacoes);
  private readonly destroyRef = inject(DestroyRef);
  private readonly nomes = new Subject<string>();

  protected readonly tipos = TIPOS_PRESCRICAO;
  protected readonly tamanhos = [10, 20, 50];

  ngOnInit(): void {
    this.nomes
      .pipe(debounceTime(350), distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
      .subscribe((nomePaciente) => this.facade.carregar({ nomePaciente, pagina: 1 }));
    this.facade.carregar();
  }

  protected rotuloTipo(tipo: TipoPrescricao): string {
    return this.tipos.find((t) => t.valor === tipo)?.rotulo ?? tipo;
  }

  protected filtrarNome(valor: string): void {
    this.nomes.next(valor.trim());
  }

  protected filtrarTipo(valor: string): void {
    this.facade.carregar({ tipo: valor as TipoPrescricao | '', pagina: 1 });
  }

  protected alternarOrdem(): void {
    const ordem = this.facade.filtro().ordem === 'desc' ? 'asc' : 'desc';
    this.facade.carregar({ ordem, pagina: 1 });
  }

  protected irPara(pagina: number): void {
    if (pagina >= 1 && pagina <= this.facade.totalPaginas()) {
      this.facade.carregar({ pagina });
    }
  }

  protected mudarTamanho(valor: string): void {
    this.facade.carregar({ tamanhoPagina: Number(valor), pagina: 1 });
  }

  protected async excluir(p: PrescricaoResumo): Promise<void> {
    if (!confirm(`Excluir a prescrição de ${p.nomePaciente}?`)) {
      return;
    }
    if (await this.facade.excluir(p.id)) {
      this.notificacoes.sucesso('Prescrição excluída.');
    }
  }
}
