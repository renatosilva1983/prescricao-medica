import { Component, inject } from '@angular/core';

import { Notificacoes } from './notificacoes';

@Component({
  selector: 'app-mensagens',
  template: `
    <div class="mensagens" aria-live="polite">
      @for (n of notificacoes.itens(); track n.id) {
        <div class="mensagem" [class.erro]="n.tipo === 'erro'" [class.sucesso]="n.tipo === 'sucesso'">
          <span>{{ n.mensagem }}</span>
          <button type="button" class="fechar" (click)="notificacoes.remover(n.id)" aria-label="Fechar">×</button>
        </div>
      }
    </div>
  `,
})
export class Mensagens {
  protected readonly notificacoes = inject(Notificacoes);
}
