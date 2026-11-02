import { Routes } from '@angular/router';

export const participationRoutes: Routes = [
  {
    path:          '',
    title:         'My Participation',
    loadComponent: () =>
      import('./pages/my-participation/my-participation')
        .then(m => m.MyParticipation)
  },
  {
    path:          'goals',
    title:         'My Goals',
    loadComponent: () =>
      import('./pages/goals/goals')
        .then(m => m.Goals)
  },
  {
    path:          'goals/new',
    title:         'New Goal',
    loadComponent: () =>
      import('./components/goal-form/goal-form')
        .then(m => m.GoalForm)
  },
];
