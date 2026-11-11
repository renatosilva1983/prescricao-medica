import { HttpErrorResponse } from '@angular/common/http';

/** RFC 7807 ProblemDetails, com as extensões de regra de negócio usadas pela API. */
export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  regra?: string;
  codigo?: string;
  errors?: Record<string, string[]>;
  extensions?: { regra?: string; codigo?: string };
}

function ehProblemDetails(valor: unknown): valor is ProblemDetails {
  return typeof valor === 'object' && valor !== null;
}

/** Monta uma mensagem legível a partir de um erro HTTP (ou qualquer erro). */
export function extrairMensagemErro(erro: unknown): string {
  if (!(erro instanceof HttpErrorResponse)) {
    return erro instanceof Error ? erro.message : 'Erro inesperado.';
  }

  if (erro.status === 0) {
    return 'Não foi possível contatar a API. Verifique se ela está em execução.';
  }

  const corpo = ehProblemDetails(erro.error) ? erro.error : undefined;
  let mensagem = corpo?.detail || corpo?.title || erro.message || `Erro HTTP ${erro.status}.`;

  if (corpo?.errors) {
    const detalhes = Object.values(corpo.errors).flat();
    if (detalhes.length) {
      mensagem += ' ' + detalhes.join(' ');
    }
  }

  if (erro.status === 422) {
    const regra = corpo?.regra ?? corpo?.extensions?.regra;
    const codigo = corpo?.codigo ?? corpo?.extensions?.codigo;
    const extras = [regra && `regra: ${regra}`, codigo && `código: ${codigo}`].filter(Boolean);
    if (extras.length) {
      mensagem += ` (${extras.join(' · ')})`;
    }
  }

  return mensagem;
}
