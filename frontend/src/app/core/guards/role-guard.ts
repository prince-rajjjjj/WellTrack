import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth';

// Usage in routes:
//   canActivate: [roleGuard],
//   data: { roles: ['Admin', 'HR'] }
// If the current user's role is NOT in the allowed list → redirect to /dashboard.
export const roleGuard: CanActivateFn = (route) => {
  const auth         = inject(AuthService);
  const allowedRoles = (route.data?.['roles'] as string[]) ?? [];

  if (allowedRoles.includes(auth.getCurrentRole())) return true;

  inject(Router).navigate(['/dashboard']);
  return false;
};
