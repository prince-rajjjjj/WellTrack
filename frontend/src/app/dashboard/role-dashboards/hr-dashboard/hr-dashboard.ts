import { Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink }              from '@angular/router';
import { AuthService }              from '../../../core/services/auth';
import { ComplianceService }        from '../../../features/compliance/services/compliance';
import { ProgramService }           from '../../../features/wellness/services/program';
import { ComplianceRecord }         from '../../../features/compliance/models/compliance.model';
import { Skeleton }                 from '../../../shared/components/skeleton/skeleton';

interface StatCard {
  label: string;
  value: number | string;
  sub:   string;
  icon:  string;
  tone:  string;
  route?: string;
}

/** HR dashboard — focused on resolving compliance issues + monitoring programs. */
@Component({
  selector:    'app-hr-dashboard',
  standalone:  true,
  imports:     [CommonModule, RouterLink, DatePipe, Skeleton],
  templateUrl: './hr-dashboard.html',
  styleUrl:    './hr-dashboard.scss',
})
export class HrDashboard implements OnInit {
  private auth          = inject(AuthService);
  readonly complianceSvc = inject(ComplianceService);
  readonly programSvc    = inject(ProgramService);

  get userName(): string { return this.auth.getCurrentName(); }
  readonly today = new Date();

  // ── Stat tiles ────────────────────────────────────────────────────
  readonly stats = computed<StatCard[]>(() => {
    const records  = this.complianceSvc.records();
    return [
      { label: 'New Issues', value: records.filter(r => r.status === 'Reported').length,
        sub: 'Awaiting your action',  icon: 'bi-inbox-fill',     tone: 'danger',  route: '/compliance/records' },
      { label: 'In Progress', value: records.filter(r => r.status === 'InProgress').length,
        sub: 'You are investigating', icon: 'bi-arrow-repeat',   tone: 'warning', route: '/compliance/records' },
      { label: 'Resolved',    value: records.filter(r => r.status === 'Closed').length,
        sub: 'Closed cases',          icon: 'bi-check-circle',   tone: 'success', route: '/compliance/records' },
      { label: 'Programs',    value: this.programSvc.programs().length,
        sub: 'Wellness programs',     icon: 'bi-calendar-event', tone: 'primary', route: '/wellness/programs' },
    ];
  });

  // ── Open issues list (top of mind for HR) ─────────────────────────
  readonly severityOrder: Record<string, number> = { Critical: 0, High: 1, Medium: 2, Low: 3 };
  readonly openIssues = computed(() =>
    this.complianceSvc.records()
      .filter(r => r.status !== 'Closed')
      .sort((a, b) => (this.severityOrder[a.severity] ?? 9) - (this.severityOrder[b.severity] ?? 9))
      .slice(0, 5)
  );

  // ── Recent resolutions ────────────────────────────────────────────
  readonly recentlyResolved = computed<ComplianceRecord[]>(() =>
    this.complianceSvc.records()
      .filter(r => r.status === 'Closed' && !!r.resolutionNotes)
      .sort((a, b) => (b.updatedAt ?? b.createdAt).localeCompare(a.updatedAt ?? a.createdAt))
      .slice(0, 5)
  );

  ngOnInit(): void {
    if (this.complianceSvc.records().length === 0) this.complianceSvc.loadRecords();
    if (this.programSvc.programs().length === 0)    this.programSvc.loadPrograms().subscribe();
  }

  severityColor(s: string): string {
    return ({ Low: 'success', Medium: 'warning', High: 'danger', Critical: 'dark' } as Record<string, string>)[s] ?? 'secondary';
  }
}
