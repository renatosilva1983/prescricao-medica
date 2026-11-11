import { Component } from '@angular/core';
import { RouterLink, RouterOutlet } from '@angular/router';

import { Mensagens } from './core/mensagens';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, Mensagens],
  template: `
    <header class="topo">
      <a routerLink="/prescricoes" class="marca">Prescrição Web</a>
    </header>
    <app-mensagens />
    <main>
      <router-outlet />
    </main>
  `,
})
export class App {}
