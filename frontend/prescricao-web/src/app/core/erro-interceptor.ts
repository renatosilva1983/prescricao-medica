import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';

import { Notificacoes } from './notificacoes';
import { extrairMensagemErro } from './problem-details';

/** Lê o ProblemDetails das respostas de erro e exibe a mensagem (`detail`; em 422 também `regra` e `codigo`). */
export const erroInterceptor: HttpInterceptorFn = (req, next) => {
  const notificacoes = inject(Notificacoes);

  return next(req).pipe(
    catchError((erro: unknown) => {
      if (erro instanceof HttpErrorResponse) {
        notificacoes.erro(extrairMensagemErro(erro));
      }
      return throwError(() => erro);
    }),
  );
};
