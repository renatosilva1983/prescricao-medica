import { DatePipe, DecimalPipe } from '@angular/common';
import { Component, OnInit, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

import { Notificacoes } from '../../../core/notificacoes';
import { PrescricaoFacade } from '../../application/prescricao-facade';
import { Prescricao, TIPOS_PRESCRICAO } from '../../domain/prescricao';

@Component({
  selector: 'app-visualizacao',
  imports: [RouterLink, DatePipe, DecimalPipe],
  templateUrl: './visualizacao.html',
})
export class Visualizacao implements OnInit {
  readonly id = input.required<string>();

  protected readonly facade = inject(PrescricaoFacade);
  private readonly router = inject(Router);
  private readonly notificacoes = inject(Notificacoes);

  protected readonly prescricao = signal<Prescricao | null>(null);

  async ngOnInit(): Promise<void> {
    this.prescricao.set(await this.facade.obter(this.id()));
  }

  protected rotuloTipo(tipo: string): string {
    return TIPOS_PRESCRICAO.find((t) => t.valor === tipo)?.rotulo ?? tipo;
  }

  protected async excluir(p: Prescricao): Promise<void> {
    if (!confirm(`Excluir a prescrição de ${p.nomePaciente}?`)) {
      return;
    }
    if (await this.facade.excluir(p.id)) {
      this.notificacoes.sucesso('Prescrição excluída.');
      await this.router.navigate(['/prescricoes']);
    }
  }
}
