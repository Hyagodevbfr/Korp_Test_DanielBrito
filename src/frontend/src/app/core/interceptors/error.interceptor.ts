import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { Toaster } from '../../features/toaster';
import { ProblemDetails } from '../models/problem-details.model';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toaster = inject(Toaster);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      toaster.error(extractErrorMessage(error));
      return throwError(() => error);
    })
  );
};

function extractErrorMessage(error: HttpErrorResponse): string {
  const problem = error.error as ProblemDetails | null;

  if (problem?.detail) {
    return problem.detail;
  }

  if (problem?.title) {
    return problem.title;
  }

  if (error.status === 0) {
    return 'Não foi possível conectar ao servidor. Verifique sua conexão.';
  }

  return `Ocorreu um erro inesperado (${error.status}).`;
}
