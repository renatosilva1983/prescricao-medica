import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import {
  AtualizarPrescricaoEntrada,
  CriarPrescricaoEntrada,
  Prescricao,
} from '../domain/prescricao';

/** Acesso REST a /api/prescricoes. */
@Injectable({ providedIn: 'root' })
export class PrescricoesApi {
  private readonly http = inject(HttpClient);
  private readonly base = '/api/prescricoes';

  obter(id: string): Observable<Prescricao> {
    return this.http.get<Prescricao>(`${this.base}/${encodeURIComponent(id)}`);
  }

  criar(entrada: CriarPrescricaoEntrada): Observable<string> {
    return this.http.post<{ id: string }>(this.base, entrada).pipe(map((r) => r.id));
  }

  atualizar(id: string, entrada: AtualizarPrescricaoEntrada): Observable<void> {
    return this.http.put<void>(`${this.base}/${encodeURIComponent(id)}`, entrada);
  }

  excluir(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${encodeURIComponent(id)}`);
  }
}
