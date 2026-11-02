import { Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink }              from '@angular/router';
import { AuthService }              from '../../../core/services/auth';
import { ComplianceService }        from '../../../features/compliance/services/compliance';
import { ComplianceRecord }         from '../../../features/compliance/models/compliance.model';
import { Skeleton }                 from '../../../shared/components/skeleton/skeleton';

interface StatCard { label: string; value: number; sub: string; icon: string; tone: string; route?: string; }

/** Compliance Officer dashboard — author audits, report issues, see HR's resolutions. */
@Component({
  selector:    'app-compliance-dashboard',
  standalone:  true,
  imports:     [CommonModule, RouterLink, DatePipe, Skeleton],
  templateUrl: './compliance-dashboard.html',
  styleUrl:    './compliance-dashboard.scss',
})
export class ComplianceDashboard implements OnInit {
  private auth          = inject(AuthService);
  readonly complianceSvc = inject(ComplianceService);

  get userName(): string { return this.auth.getCurrentName(); }
  readonly today = new Date();

  // ── Stat tiles ────────────────────────────────────────────────────
  readonly stats = computed<StatCard[]>(() => {
    const audits  = this.complianceSvc.audits();
    const records = this.complianceSvc.records();
    return [
      { label: 'Drafts',          value: audits.filter(a => a.status === 'Draft').length,
        sub: 'Not submitted yet',     icon: 'bi-pencil-square',   tone: 'secondary', route: '/compliance/audits' },
      { label: 'Submitted',       value: audits.filter(a => a.status === 'Submitted').length,
        sub: 'Awaiting Admin review', icon: 'bi-hourglass-split', tone: 'warning',   route: '/compliance/audits' },
      { label: 'Approved',        value: audits.filter(a => a.status === 'Approved').length,
        sub: 'Closed successfully',   icon: 'bi-patch-check',     tone: 'success',   route: '/compliance/audits' },
      { label: 'Issues Reported', value: records.length,
        sub: `${records.filter(r => r.status !== 'Closed').length} still open`,
        icon: 'bi-clipboard-plus', tone: 'primary', route: '/compliance/records' },
    ];
  });

  // Drafts the CO hasn't submitted yet — visible reminder
  readonly myDraftAudits = computed(() =>
    this.complianceSvc.audits().filter(a => a.status === 'Draft')
  );

  // ── Issues this CO reported that HR has now closed (the "loop closed" feed) ──
  readonly myResolvedReports = computed<ComplianceRecord[]>(() => {
    const myId = this.auth.getCurrentUserId();
    if (!myId) return [];
    return this.complianceSvc.records()
      .filter(r => r.status === 'Closed' && r.createdBy?.toLowerCase() === myId.toLowerCase())
      .sort((a, b) => (b.updatedAt ?? b.createdAt).localeCompare(a.updatedAt ?? a.createdAt))
      .slice(0, 5);
  });

  ngOnInit(): void {
    if (this.complianceSvc.audits().length === 0)  this.complianceSvc.loadAudits();
    if (this.complianceSvc.records().length === 0) this.complianceSvc.loadRecords();
  }
}
