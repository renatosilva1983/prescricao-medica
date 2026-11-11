import { HttpErrorResponse } from '@angular/common/http';
import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';

import { ConsultasOData } from '../data/consultas-odata';
import { PrescricoesApi } from '../data/prescricoes-api';
import { CriarPrescricaoEntrada, PrescricaoResumo } from '../domain/prescricao';
import { PrescricaoFacade } from './prescricao-facade';

function resumo(id: string): PrescricaoResumo {
  return {
    id,
    pacienteId: 'p1',
    nomePaciente: 'Ana Souza',
    tipo: 'NaoEstruturada',
    nomeProfissional: 'Dr. João',
    registroProfissional: 'CRM-SP 123456',
    emitidaEmUtc: '2026-11-10T12:00:00Z',
    atualizadaEmUtc: null,
    quantidadeItens: 0,
  };
}

const entrada: CriarPrescricaoEntrada = {
  pacienteId: 'p1',
  tipo: 'NaoEstruturada',
  nomeProfissional: 'Dr. João',
  registroProfissional: 'CRM-SP 123456',
  textoLivre: 'Repouso.',
};

describe('PrescricaoFacade', () => {
  let facade: PrescricaoFacade;
  let api: { obter: ReturnType<typeof vi.fn>; criar: ReturnType<typeof vi.fn>; atualizar: ReturnType<typeof vi.fn>; excluir: ReturnType<typeof vi.fn> };
  let odata: { listarPrescricoes: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    api = { obter: vi.fn(), criar: vi.fn(), atualizar: vi.fn(), excluir: vi.fn() };
    odata = { listarPrescricoes: vi.fn().mockReturnValue(of({ total: 25, itens: [resumo('1'), resumo('2')] })) };

    TestBed.configureTestingModule({
      providers: [
        { provide: PrescricoesApi, useValue: api },
        { provide: ConsultasOData, useValue: odata },
      ],
    });
    facade = TestBed.inject(PrescricaoFacade);
  });

  it('carrega a lista e o total aplicando o filtro', async () => {
    await facade.carregar({ nomePaciente: 'ana', pagina: 2 });

    expect(odata.listarPrescricoes).toHaveBeenCalledWith(
      expect.objectContaining({ nomePaciente: 'ana', pagina: 2, ordem: 'desc', tamanhoPagina: 10 }),
    );
    expect(facade.lista().map((p) => p.id)).toEqual(['1', '2']);
    expect(facade.total()).toBe(25);
    expect(facade.totalPaginas()).toBe(3);
    expect(facade.carregando()).toBe(false);
    expect(facade.erro()).toBeNull();
  });

  it('expõe a mensagem do ProblemDetails em erro quando a consulta falha', async () => {
    odata.listarPrescricoes.mockReturnValue(
      throwError(() => new HttpErrorResponse({ status: 500, error: { title: 'Falha', detail: 'Banco indisponível.' } })),
    );

    await facade.carregar();

    expect(facade.erro()).toBe('Banco indisponível.');
    expect(facade.lista()).toEqual([]);
    expect(facade.carregando()).toBe(false);
  });

  it('salvar sem id cria (POST) e devolve o novo id', async () => {
    api.criar.mockReturnValue(of('novo-id'));

    const id = await facade.salvar(entrada);

    expect(id).toBe('novo-id');
    expect(api.criar).toHaveBeenCalledWith(entrada);
    expect(api.atualizar).not.toHaveBeenCalled();
  });

  it('salvar com id atualiza (PUT) sem enviar pacienteId', async () => {
    api.atualizar.mockReturnValue(of(undefined));

    const id = await facade.salvar(entrada, 'abc');

    expect(id).toBe('abc');
    const corpo = api.atualizar.mock.calls[0][1];
    expect(api.atualizar.mock.calls[0][0]).toBe('abc');
    expect(corpo).not.toHaveProperty('pacienteId');
    expect(corpo).toMatchObject({ tipo: 'NaoEstruturada', textoLivre: 'Repouso.' });
  });

  it('salvar com 422 inclui regra e código na mensagem de erro', async () => {
    api.criar.mockReturnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            status: 422,
            error: { detail: 'Prescrição estruturada precisa de itens.', regra: 'ItensObrigatorios', codigo: 'PRS-001' },
          }),
      ),
    );

    const id = await facade.salvar(entrada);

    expect(id).toBeNull();
    expect(facade.erro()).toBe('Prescrição estruturada precisa de itens. (regra: ItensObrigatorios · código: PRS-001)');
    expect(facade.salvando()).toBe(false);
  });

  it('excluir remove e recarrega; volta uma página se a atual ficar vazia', async () => {
    odata.listarPrescricoes.mockReturnValue(of({ total: 11, itens: [resumo('11')] }));
    await facade.carregar({ pagina: 2 });
    api.excluir.mockReturnValue(of(undefined));
    odata.listarPrescricoes.mockReturnValue(of({ total: 10, itens: [resumo('1')] }));

    const ok = await facade.excluir('11');

    expect(ok).toBe(true);
    expect(api.excluir).toHaveBeenCalledWith('11');
    expect(facade.filtro().pagina).toBe(1);
    expect(facade.total()).toBe(10);
  });
});
