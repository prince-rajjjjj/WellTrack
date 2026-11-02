import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../../core/services/auth';
import { EngagementService } from '../../services/engagement';
import { ProgramService }   from '../../../wellness/services/program';
import { ActivityService }  from '../../../wellness/services/activity';
import {
  EmployeeEngagementDto,
  OrgSummaryDto,
  FactorHealthDto,
  ProgramHealthDto,
} from '../../models/engagement.model';
import { ActivityDto } from '../../../wellness/models/activity.model';

@Component({
  selector: 'app-engagement-tracker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './engagement-tracker.html',
  styleUrl: './engagement-tracker.scss',
})

export class EngagementTracker implements OnInit {
  private svc         = inject(EngagementService);
  private programSvc  = inject(ProgramService);
  private activitySvc = inject(ActivityService);
  readonly auth       = inject(AuthService);

  // ── Role shortcuts ────────────────────────────────────────────────
  get isHR():       boolean { return this.auth.isHROrAdmin(); }
  get isEmployee(): boolean { return this.auth.isEmployee(); }

  // ── Shared loading / error ────────────────────────────────────────
  loading = signal(false);
  error   = signal('');

  // ── Employee view ─────────────────────────────────────────────────
  myEngagement = signal<EmployeeEngagementDto[]>([]);

  // ── HR analytics signals ──────────────────────────────────────────
  orgSummary    = signal<OrgSummaryDto | null>(null);
  factorHealth  = signal<FactorHealthDto | null>(null);
  programHealth = signal<ProgramHealthDto[]>([]);

  // ── Pagination — employee cards ───────────────────────────────────
  empPage     = signal(1);
  empPageSize = 9;
  empPages    = computed(() => Math.max(1, Math.ceil(this.myEngagement().length / this.empPageSize)));
  pagedEngagement = computed(() => {
    const start = (this.empPage() - 1) * this.empPageSize;
    return this.myEngagement().slice(start, start + this.empPageSize);
  });
  empGoToPage(p: number): void { if (p >= 1 && p <= this.empPages()) this.empPage.set(p); }
  empPageNumbers(): number[] {
    const t = this.empPages(), c = this.empPage(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  // ── Tips panel ───────────────────────────────────────────────────
  showTips = signal(false);
  toggleTips(): void { this.showTips.update(v => !v); }
  allTips = computed(() => {
    const seen = new Set<string>();
    return this.myEngagement().flatMap(r => r.improvementTips).filter(t => !seen.has(t) && seen.add(t));
  });

  // ── Sync Scores state ─────────────────────────────────────────────
  syncing     = signal(false);
  syncMessage = signal('');

  // ── Name lookup map ───────────────────────────────────────────────
  private programMap: Record<string, string> = {};

  programName(id: string): string { return this.programMap[id] ?? `Program · ${this.shortId(id)}`; }

  // ── Lifecycle ─────────────────────────────────────────────────────
  ngOnInit(): void {
    if (this.isHR) {
      this.loadHrDashboard();
    } else {
      this.loadMyEngagement();
      if (this.programSvc.programs().length === 0)
        this.programSvc.loadPrograms().subscribe();
    }
  }

  // ── Data loaders ──────────────────────────────────────────────────
  private loadMyEngagement(): void {
    this.loading.set(true);
    this.svc.getMyEngagement().subscribe({
      next: res => {
        this.loading.set(false);
        if (res.succeeded) this.myEngagement.set(res.data ?? []);
        else this.error.set(res.message ?? 'Failed to load engagement data.');
      },
      error: () => { this.loading.set(false); this.error.set('Request failed.'); },
    });
  }

  private loadHrDashboard(): void {
    this.loading.set(true);

    // Load program name cache — non-blocking
    if (this.programSvc.programs().length === 0) {
      this.programSvc.loadPrograms().subscribe({
        next: () => {
          for (const p of this.programSvc.programs()) this.programMap[p.id] = p.title;
        }
      });
    } else {
      for (const p of this.programSvc.programs()) this.programMap[p.id] = p.title;
    }

    // Load all analytics in parallel
    let pending = 2;
    const done  = () => { if (--pending === 0) this.loading.set(false); };

    this.svc.getOrgSummary().subscribe({
      next: r => { if (r.succeeded) this.orgSummary.set(r.data ?? null); done(); },
      error: () => done(),
    });
    this.svc.getFactorHealth().subscribe({
      next: r => { if (r.succeeded) this.factorHealth.set(r.data ?? null); done(); },
      error: () => done(),
    });
    this.svc.getProgramHealth().subscribe({
      next: r => { if (r.succeeded) this.programHealth.set(r.data ?? []); },
      error: () => {},
    });
  }

  // ── Sync all scores ───────────────────────────────────────────────
  syncAll(): void {
    this.syncing.set(true);
    this.syncMessage.set('');
    this.svc.recalculateAll().subscribe({
      next: res => {
        this.syncing.set(false);
        this.syncMessage.set(
          res.succeeded
            ? `✓ ${res.data?.updated ?? 0} record(s) synced`
            : res.message ?? 'Sync failed.'
        );
        if (res.succeeded) this.loadHrDashboard();
        setTimeout(() => this.syncMessage.set(''), 4000);
      },
      error: () => {
        this.syncing.set(false);
        this.syncMessage.set('Sync failed — server error.');
        setTimeout(() => this.syncMessage.set(''), 4000);
      },
    });
  }

  // ── Factor health helpers ─────────────────────────────────────────
  factors(): { label: string; avg: number; pct: number; max: number; color: string }[] {
    const f = this.factorHealth();
    if (!f) return [];
    return [
      { label: 'Attendance',  avg: f.attendanceAvg,  pct: f.attendancePct,  max: 30, color: '#4CAF50' },
      { label: 'Activities',  avg: f.activityAvg,    pct: f.activityPct,    max: 25, color: '#2196F3' },
      { label: 'Goals',       avg: f.goalAvg,        pct: f.goalPct,        max: 25, color: '#9C27B0' },
      { label: 'Feedback',    avg: f.feedbackAvg,    pct: f.feedbackPct,    max: 10, color: '#FF9800' },
      { label: 'Consistency', avg: f.consistencyAvg, pct: f.consistencyPct, max: 10, color: '#00BCD4' },
    ];
  }

  factorWeak(pct: number): boolean { return pct < 50; }

  // ── Program drill-down (activity-level) ──────────────────────────
  expandedProgramId = signal<string | null>(null);
  private programActivityMap = signal<Record<string, ActivityDto[]>>({});
  loadingProgramId  = signal<string | null>(null);

  toggleProgram(programId: string): void {
    if (this.expandedProgramId() === programId) {
      this.expandedProgramId.set(null);
      return;
    }
    if (this.programActivityMap()[programId]) {
      this.expandedProgramId.set(programId);
      return;
    }
    this.loadingProgramId.set(programId);
    this.activitySvc.loadActivities(programId).subscribe({
      next: activities => {
        this.programActivityMap.update(m => ({ ...m, [programId]: activities }));
        this.expandedProgramId.set(programId);
        this.loadingProgramId.set(null);
      },
      error: () => this.loadingProgramId.set(null),
    });
  }

  programActivities(programId: string): ActivityDto[] {
    return this.programActivityMap()[programId] ?? [];
  }

  attendancePct(activity: ActivityDto): number {
    return activity.enrolledCount > 0
      ? Math.round(activity.attendedCount / activity.enrolledCount * 100)
      : 0;
  }

  // ── Program health helpers ────────────────────────────────────────
  programBarWidth(count: number, total: number): string {
    return total > 0 ? `${Math.round(count / total * 100)}%` : '0%';
  }

  // ── Shared helpers ────────────────────────────────────────────────
  scoreColorClass(score: number): string {
    if (score >= 75) return 'text-success';
    if (score >= 40) return 'text-warning';
    return 'text-primary';
  }

  shortId(id: string): string { return id.substring(0, 8).toUpperCase(); }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-US', {
      month: 'short', day: 'numeric', year: 'numeric',
    });
  }
}
