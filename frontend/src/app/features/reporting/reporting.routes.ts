import { Routes } from '@angular/router';

export const reportingRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/overview-report/overview-report').then(m => m.OverviewReport)
  },
  {
    path: 'participation',
    loadComponent: () => import('./pages/participation-report/participation-report').then(m => m.ParticipationReport)
  },
  {
    path: 'engagement',
    loadComponent: () => import('./pages/goal-report/goal-report').then(m => m.GoalReport)
  }
];