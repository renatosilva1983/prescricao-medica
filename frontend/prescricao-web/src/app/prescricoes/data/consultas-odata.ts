import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';

import {
  FiltroPrescricoes,
  Medicamento,
  Paciente,
  Pagina,
  PrescricaoResumo,
  TAMANHO_MAXIMO_PAGINA,
} from '../domain/prescricao';

interface RespostaOData<T> {
  '@odata.count'?: number;
  value: T[];
}

/** Escapa um literal de string OData (aspas simples são duplicadas). */
export function literalOData(valor: string): string {
  return `'${valor.replace(/'/g, "''")}'`;
}

/** Monta o $filter da listagem de prescrições. */
export function montarFiltroPrescricoes(filtro: Pick<FiltroPrescricoes, 'nomePaciente' | 'tipo'>): string {
  const partes: string[] = [];
  const nome = filtro.nomePaciente.trim();
  if (nome) {
    partes.push(`contains(nomePaciente,${literalOData(nome)})`);
  }
  if (filtro.tipo) {
    partes.push(`tipo eq ${literalOData(filtro.tipo)}`);
  }
  return partes.join(' and ');
}

/** Consultas de leitura via OData (propriedades camelCase, página máx. 50). */
@Injectable({ providedIn: 'root' })
export class ConsultasOData {
  private readonly http = inject(HttpClient);
  private readonly base = '/odata';

  listarPrescricoes(filtro: FiltroPrescricoes): Observable<Pagina<PrescricaoResumo>> {
    const top = Math.min(Math.max(filtro.tamanhoPagina, 1), TAMANHO_MAXIMO_PAGINA);
    const skip = Math.max(filtro.pagina - 1, 0) * top;

    let params = new HttpParams()
      .set('$orderby', `emitidaEmUtc ${filtro.ordem}`)
      .set('$top', top)
      .set('$skip', skip)
      .set('$count', true);

    const filtroOData = montarFiltroPrescricoes(filtro);
    if (filtroOData) {
      params = params.set('$filter', filtroOData);
    }

    return this.http
      .get<RespostaOData<PrescricaoResumo>>(`${this.base}/Prescricoes`, { params })
      .pipe(map((r) => ({ total: r['@odata.count'] ?? r.value.length, itens: r.value })));
  }

  buscarPacientes(nome: string, limite = 10): Observable<Paciente[]> {
    const params = new HttpParams()
      .set('$filter', `contains(nomeCompleto,${literalOData(nome.trim())})`)
      .set('$orderby', 'nomeCompleto')
      .set('$top', limite);
    return this.http
      .get<RespostaOData<Paciente>>(`${this.base}/Pacientes`, { params })
      .pipe(map((r) => r.value));
  }

  buscarMedicamentos(nome: string, limite = 10): Observable<Medicamento[]> {
    const params = new HttpParams()
      .set('$filter', `contains(nome,${literalOData(nome.trim())})`)
      .set('$orderby', 'nome')
      .set('$top', limite);
    return this.http
      .get<RespostaOData<Medicamento>>(`${this.base}/Medicamentos`, { params })
      .pipe(map((r) => r.value));
  }
}
