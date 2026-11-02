import { Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink }              from '@angular/router';
import { AuthService }              from '../../../core/services/auth';
import { ComplianceService }        from '../../../features/compliance/services/compliance';
import { ProgramService }           from '../../../features/wellness/services/program';
import { Skeleton }                 from '../../../shared/components/skeleton/skeleton';

interface StatCard { label: string; value: number; sub: string; icon: string; tone: string; route?: string; }

/** Admin dashboard — oversight role: review audits, watch issues, govern policies. */
@Component({
  selector:    'app-admin-dashboard',
  standalone:  true,
  imports:     [CommonModule, RouterLink, DatePipe, Skeleton],
  templateUrl: './admin-dashboard.html',
  styleUrl:    './admin-dashboard.scss',
})
export class AdminDashboard implements OnInit {
  private auth          = inject(AuthService);
  readonly complianceSvc = inject(ComplianceService);
  readonly programSvc    = inject(ProgramService);

  get userName(): string { return this.auth.getCurrentName(); }
  readonly today = new Date();

  // ── Stat tiles ────────────────────────────────────────────────────
  readonly stats = computed<StatCard[]>(() => {
    const audits   = this.complianceSvc.audits();
    const records  = this.complianceSvc.records();
    return [
      { label: 'Audits to Review', value: audits.filter(a => a.status === 'Submitted').length,
        sub: 'Approve or reject', icon: 'bi-patch-check',       tone: 'warning', route: '/compliance/audits' },
      { label: 'Open Issues',      value: records.filter(r => r.status !== 'Closed').length,
        sub: `${records.length} total reported`,
        icon: 'bi-eye-fill',       tone: 'info',    route: '/compliance/records' },
      { label: 'Active Policies',  value: this.complianceSvc.policies().filter(p => p.status === 'Active').length,
        sub: `${this.complianceSvc.policies().length} total`,
        icon: 'bi-file-earmark-lock', tone: 'primary', route: '/compliance/policies' },
      { label: 'Programs',         value: this.programSvc.programs().length,
        sub: 'Across the organization',
        icon: 'bi-calendar-event', tone: 'success', route: '/wellness/programs' },
    ];
  });

  readonly pendingAudits = computed(() =>
    this.complianceSvc.audits().filter(a => a.status === 'Submitted')
  );

  ngOnInit(): void {
    if (this.complianceSvc.audits().length === 0)   this.complianceSvc.loadAudits();
    if (this.complianceSvc.records().length === 0)  this.complianceSvc.loadRecords();
    if (this.complianceSvc.policies().length === 0) this.complianceSvc.loadPolicies();
    if (this.programSvc.programs().length === 0)    this.programSvc.loadPrograms().subscribe();
  }
}
