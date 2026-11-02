import { Routes } from '@angular/router';

export const engagementRoutes: Routes = [
  { path: '', redirectTo: 'tracker', pathMatch: 'full' },
  {
    path: 'tracker',
    loadComponent: () =>
      import('./pages/engagement-tracker/engagement-tracker').then(
        (m) => m.EngagementTracker
      ),
  },
  {
    path: 'feedback',
    loadComponent: () =>
      import('./pages/feedback/feedback').then((m) => m.Feedback),
  },
];
