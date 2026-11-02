import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/role-guard';

export const complianceRoutes: Routes = [
  { path: '', redirectTo: 'records', pathMatch: 'full' },
  {
    path: 'policies',
    // Admin + ComplianceOfficer + HR can view policies (HR is read-only)
    loadComponent: () => import('./components/policies/policies').then(m => m.Policies)
  },
  {
    path: 'records',
    // Admin + ComplianceOfficer + HR can view reported issues
    loadComponent: () => import('./components/compliance-records/compliance-records').then(m => m.ComplianceRecords)
  },
  {
    path: 'audits',
    // Audits are not visible to HR — only Admin (review) + ComplianceOfficer (log)
    canActivate: [roleGuard],
    data: { roles: ['Admin', 'ComplianceOfficer'] },
    loadComponent: () => import('./components/audit-logs/audit-logs').then(m => m.AuditLogs)
  },
];
