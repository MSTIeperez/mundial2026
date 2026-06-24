import { Routes } from '@angular/router';
import { authGuard } from './services/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'grupos', pathMatch: 'full' },
  {
    path: 'login',
    loadComponent: () => import('./pages/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'grupos',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/grupos/grupos.component').then(m => m.GruposComponent)
  },
  {
    path: 'eliminatorias',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/eliminatorias/eliminatorias.component').then(m => m.EliminatoriasComponent)
  },
  { path: '**', redirectTo: 'grupos' }
];
