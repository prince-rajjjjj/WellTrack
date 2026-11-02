import { Routes } from '@angular/router';

export const wellnessRoutes: Routes = [
  { path: '', redirectTo: 'programs', pathMatch: 'full' },
  { path: 'programs',          loadComponent: () => import('./pages/programs/programs').then(m => m.Programs) },
  { path: 'activities',        loadComponent: () => import('./pages/activities/activities').then(m => m.Activities) },
  { path: 'activities/:id',    loadComponent: () => import('./pages/activity-detail/activity-detail').then(m => m.ActivityDetail) },
];
