import { Component, inject } from '@angular/core';
import { CommonModule }       from '@angular/common';
import { AuthService }        from '../core/services/auth';

import { AdminDashboard }      from './role-dashboards/admin-dashboard/admin-dashboard';
import { HrDashboard }         from './role-dashboards/hr-dashboard/hr-dashboard';
import { ComplianceDashboard } from './role-dashboards/compliance-dashboard/compliance-dashboard';
import { CoachDashboard }      from './role-dashboards/coach-dashboard/coach-dashboard';
import { EmployeeDashboard }   from './role-dashboards/employee-dashboard/employee-dashboard';

/**
 * Thin shell: reads the current user's role and renders the matching
 * role-specific dashboard component. Each role component owns its data
 * loading, signals and template — keeps things separated so HR-only
 * changes never accidentally break Admin's view (and vice-versa).
 */
@Component({
  selector:   'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    AdminDashboard, HrDashboard, ComplianceDashboard, CoachDashboard, EmployeeDashboard,
  ],
  templateUrl: './dashboard.html',
})
export class Dashboard {
  private auth = inject(AuthService);
  get role(): string { return this.auth.getCurrentRole() || 'Employee'; }
}
