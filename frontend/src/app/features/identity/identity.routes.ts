import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role-guard';

/**
 * PUBLIC routes — loaded inside AuthLayout (no sidebar/navbar).
 * Member 1: Add your login and register components here.
 */
export const identityPublicRoutes: Routes = [
  // Example stub — replace with real login component:
  // { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: '', loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent) },
];

/**
 * PROTECTED routes — loaded inside MainLayout (requires authGuard).
 * Member 1: Add your profile and user-management pages here.
 * Path prefix: /identity/...
 */
export const identityRoutes: Routes = [
  // Example stub — replace with real profile component:
  { path: 'profile', loadComponent: () => import('./pages/profile/profile').then(m => m.ProfileComponent) },
  {
    path: 'users',
    loadComponent: () => import('./pages/user-management/user-management').then(m => m.UserManagementComponent)
  },
  {
    path: 'audit-logs',
    loadComponent: () => import('./pages/audit-logs/audit-logs').then(m => m.AuditLogsComponent),
    canActivate: [roleGuard],
    data: { roles: ['Admin'] }
  }
];
