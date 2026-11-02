import { Routes } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout';
import { AuthLayout } from './layout/auth-layout/auth-layout';
import { authGuard }  from './core/guards/auth-guard';
import { roleGuard }  from './core/guards/role-guard';

export const routes: Routes = [

  // Default redirect
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // ── AUTH LAYOUT ─────────────────────────────────────────────────
  // Pages without sidebar/navbar (login, register)
  // Member 1 (Identity): add your login/register components to identity.routes.ts
  {
    path: '',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        loadChildren: () =>
          import('./features/identity/identity.routes')
            .then((m) => m.identityPublicRoutes),
      },
    ],
  },

  // ── MAIN LAYOUT ─────────────────────────────────────────────────
  // All protected pages — guarded by authGuard
  {
    path: '',
    component: MainLayout,
    canActivate: [authGuard],
    children: [

      // Dashboard — Tech Lead
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'dashboard/affected-programs',
        canActivate: [roleGuard],
        data: { roles: ['HR', 'Admin'] },
        loadComponent: () =>
          import('./dashboard/affected-programs/affected-programs').then((m) => m.AffectedPrograms),
      },

      // Identity protected pages (profile, user management) — Member 1
      {
        path: 'identity',
        loadChildren: () =>
          import('./features/identity/identity.routes')
            .then((m) => m.identityRoutes),
      },

      // Wellness (programs, activities, sessions) — Member 2
      {
        path: 'wellness',
        loadChildren: () =>
          import('./features/wellness/wellness.routes')
            .then((m) => m.wellnessRoutes),
      },

      // Participation (goals, my-participation, recommendations) — Member 3
      {
        path: 'participation',
        loadChildren: () =>
          import('./features/participation/participation.routes')
            .then((m) => m.participationRoutes),
      },

      // Engagement (tracker, feedback) — Member 4
      {
        path: 'engagement',
        loadChildren: () =>
          import('./features/engagement/engagement.routes')
            .then((m) => m.engagementRoutes),
      },

      // Compliance — Admin, ComplianceOfficer (full); HR (policies + records only, enforced per-route)
      {
        path: 'compliance',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'ComplianceOfficer', 'HR'] },
        loadChildren: () =>
          import('./features/compliance/compliance.routes')
            .then((m) => m.complianceRoutes),
      },

      // Reporting — Member 6 (HR + Admin only)
      {
        path: 'reporting',
        canActivate: [roleGuard],
        data: { roles: ['Admin', 'HR'] },
        loadChildren: () =>
          import('./features/reporting/reporting.routes')
            .then((m) => m.reportingRoutes),
      },

      // Notification — Member 7
      {
        path: 'notifications',
        loadChildren: () =>
          import('./features/notification/notification.routes')
            .then((m) => m.notificationRoutes),
      },
    ],
  },

  // Catch-all
  { path: '**', redirectTo: 'login' },
];
