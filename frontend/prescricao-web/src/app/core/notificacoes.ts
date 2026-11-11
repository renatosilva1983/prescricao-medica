import { Injectable, signal } from '@angular/core';

export interface Notificacao {
  id: number;
  tipo: 'erro' | 'sucesso';
  mensagem: string;
}

@Injectable({ providedIn: 'root' })
export class Notificacoes {
  private proximoId = 1;
  readonly itens = signal<Notificacao[]>([]);

  erro(mensagem: string): void {
    this.adicionar('erro', mensagem, 8000);
  }

  sucesso(mensagem: string): void {
    this.adicionar('sucesso', mensagem, 4000);
  }

  remover(id: number): void {
    this.itens.update((lista) => lista.filter((n) => n.id !== id));
  }

  private adicionar(tipo: Notificacao['tipo'], mensagem: string, duracaoMs: number): void {
    const id = this.proximoId++;
    this.itens.update((lista) => [...lista, { id, tipo, mensagem }]);
    setTimeout(() => this.remover(id), duracaoMs);
  }
}
