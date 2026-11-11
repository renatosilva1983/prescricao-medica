import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'prescricoes' },
  {
    path: 'prescricoes',
    children: [
      {
        path: '',
        title: 'Prescrições',
        loadComponent: () => import('./prescricoes/presentation/lista/lista').then((m) => m.Lista),
      },
      {
        path: 'nova',
        title: 'Nova prescrição',
        loadComponent: () => import('./prescricoes/presentation/formulario/formulario').then((m) => m.Formulario),
      },
      {
        path: ':id',
        title: 'Prescrição',
        loadComponent: () =>
          import('./prescricoes/presentation/visualizacao/visualizacao').then((m) => m.Visualizacao),
      },
      {
        path: ':id/editar',
        title: 'Editar prescrição',
        loadComponent: () => import('./prescricoes/presentation/formulario/formulario').then((m) => m.Formulario),
      },
    ],
  },
  { path: '**', redirectTo: 'prescricoes' },
];
