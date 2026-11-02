import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplianceService } from '../../services/compliance';
import { ProgramService } from '../../../wellness/services/program';
import { ActivityService } from '../../../wellness/services/activity';
import { AuthService } from '../../../../core/services/auth';
import { ComplianceRecord, CreateComplianceRecordDto } from '../../models/compliance.model';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';

@Component({
  selector:    'app-compliance-records',
  standalone:  true,
  imports:     [CommonModule, FormsModule, Skeleton],
  templateUrl: './compliance-records.html',
  styleUrl:    './compliance-records.scss',
})
export class ComplianceRecords implements OnInit {
  readonly svc         = inject(ComplianceService);
  readonly programSvc  = inject(ProgramService);
  readonly activitySvc = inject(ActivityService);
  private  auth        = inject(AuthService);

  // Role-driven capabilities — keep UI in lockstep with backend authz
  // (ComplianceController: report → CO, start/close → HR, view → all three).
  get canReport():  boolean { return this.auth.getCurrentRole() === 'ComplianceOfficer'; }
  get canResolve(): boolean { return this.auth.getCurrentRole() === 'HR'; }
  get isAdmin():    boolean { return this.auth.getCurrentRole() === 'Admin'; }

  // ── Role-aware page chrome ─────────────────────────────────────────
  // Each role gets a distinct title, subtitle and intent banner so the
  // same URL feels purpose-built for the user logged in.
  get pageTitle(): string {
    if (this.isAdmin)    return 'Issue Oversight';
    if (this.canReport)  return 'Report Compliance Issues';
    if (this.canResolve) return 'Resolve Compliance Issues';
    return 'Compliance Records';
  }

  get pageSubtitle(): string {
    const open = this.svc.openCount();
    if (this.isAdmin)    return `${open} open issue${open === 1 ? '' : 's'} across the organization`;
    if (this.canReport)  return `${open} of your issues still open`;
    if (this.canResolve) return `${open} open issue${open === 1 ? '' : 's'} need attention`;
    return `${open} open issues`;
  }

  // Intent banner — the colored card right under the header.
  get bannerStyle(): { bg: string; icon: string; tone: string } {
    if (this.isAdmin)    return { bg: 'linear-gradient(135deg, #f3f4f6, #e5e7eb)', icon: 'bi-eye-fill',         tone: 'text-secondary' };
    if (this.canReport)  return { bg: 'linear-gradient(135deg, #e7f5ff, #cfe7fa)', icon: 'bi-clipboard-plus',   tone: 'text-primary'   };
    if (this.canResolve) return { bg: 'linear-gradient(135deg, #e6f9f0, #c8f1de)', icon: 'bi-clipboard-check',  tone: 'text-success'   };
    return { bg: '#f8f9fa', icon: 'bi-info-circle', tone: 'text-muted' };
  }

  get bannerHeadline(): string {
    if (this.isAdmin)    return 'Read-only oversight view';
    if (this.canReport)  return 'Raise new compliance issues here';
    if (this.canResolve) return 'Move issues through the resolution workflow';
    return '';
  }

  get bannerDescription(): string {
    if (this.isAdmin)
      return 'Inspect any case via the details icon. You cannot create, start, or close issues — that lives with Compliance & HR.';
    if (this.canReport)
      return 'Use “Report Issue” for any compliance gap you discover. HR will pick it up and drive it to closure.';
    if (this.canResolve)
      return 'Click Start when you begin investigating, then Close once the issue is resolved.';
    return '';
  }

  filterStatus = signal<string>('All');
  statusTabs   = ['All', 'Reported', 'InProgress', 'Closed'];
  severities   = ['Low', 'Medium', 'High', 'Critical'];

  showForm = signal(false);
  form: CreateComplianceRecordDto = this._blank();

  // Details-modal state — any role can open it (read-only deep-dive).
  selectedRecord = signal<ComplianceRecord | null>(null);
  openDetails(r: ComplianceRecord):  void { this.selectedRecord.set(this.enrich(r)); }
  closeDetails():                    void { this.selectedRecord.set(null); }

  /**
   * Fills missing programName / activityName / policyName on a record by
   * looking them up from the locally-cached programs / activities / policies
   * signals. Backstop for legacy records whose snapshot columns are null —
   * the user still gets meaningful names instead of "—".
   */
  private enrich(r: ComplianceRecord): ComplianceRecord {
    const programLookup  = (id: string) =>
      this.programSvc.programs().find(p => p.id === id)?.title ?? '';
    const activityLookup = (id: string | null) =>
      id ? (this.activitySvc.activities().find(a => a.id === id)?.title ?? '') : '';
    const policyLookup   = (id: string) =>
      this.svc.policies().find(p => p.id === id)?.policyName ?? '';

    return {
      ...r,
      programName:  r.programName  || programLookup(r.programId),
      activityName: r.activityName || activityLookup(r.activityId),
      policyName:   r.policyName   || policyLookup(r.policyId),
    };
  }

  ngOnInit(): void {
    // Full records list keeps openCount + cross-page lookups accurate.
    if (this.svc.records().length === 0) this.svc.loadRecords();
    // Paged records drive the table display.
    this.svc.loadPagedRecords(1, this.pageSize, this.filterStatus());
    // Always refresh policies + programs on entry — items deleted on other
    // pages might still be cached in the signals from a previous visit.
    this.svc.loadPolicies();
    this.programSvc.loadPrograms().subscribe();
    // Activities are needed for the enrich() fallback lookup. Loaded once;
    // the modal & table use whatever cache is present.
    if (this.activitySvc.activities().length === 0) {
      this.activitySvc.loadActivities().subscribe();
    }
  }

  onProgramChange(programId: string): void {
    this.form.activityId = null;
    // Re-fetch activities for the selected program so a freshly-deleted
    // activity doesn't leak in from the cached signal.
    if (programId) this.activitySvc.loadActivities(programId).subscribe();
  }

  // ── Dropdown sources (filtered to exclude deleted / archived / inactive) ──
  // Programs that are still meaningful for issue reporting:
  // exclude clearly-terminal states (Cancelled, Archived, Inactive).
  get selectablePrograms() {
    const bad = new Set(['Cancelled', 'Archived', 'Inactive']);
    return this.programSvc.programs().filter(p => !bad.has(p.status));
  }

  // Activities for the currently-selected program, excluding cancelled / archived.
  get programActivities() {
    const bad = new Set(['Cancelled', 'Archived', 'Inactive']);
    return this.activitySvc.activities().filter(a =>
      a.programID === this.form.programId && !bad.has(String(a.status))
    );
  }

  // Only Active policies should be linkable to a new issue.
  get selectablePolicies() {
    return this.svc.policies().filter(p => p.status === 'Active');
  }

  // ── Pagination (server-side) ──────────────────────────────────────
  // pagedRecords transforms the service's recordsPage() through enrich()
  // so program/activity/policy names show up even on legacy rows.
  page     = signal(1);
  pageSize = 10;

  pagedRecords = computed(() => this.svc.recordsPage().map(r => this.enrich(r)));
  totalCount   = computed(() => this.svc.recordsTotalCount());
  totalPages   = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.svc.loadPagedRecords(p, this.pageSize, this.filterStatus());
  }
  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  setFilter(s: string): void {
    this.filterStatus.set(s);
    this.page.set(1);
    this.svc.loadPagedRecords(1, this.pageSize, s);
  }

  openForm():  void {
    this.form = this._blank();
    // Force-refresh programs & policies right before showing the form so the
    // user always picks from the latest source of truth. The dropdowns are
    // bound to signals from the singleton services, so once these HTTP calls
    // resolve the dropdowns auto-update — no manual refresh needed.
    this.programSvc.loadPrograms().subscribe();
    this.svc.loadPolicies();
    this.showForm.set(true);
  }
  closeForm(): void { this.showForm.set(false); }

  submit(): void {
    if (!this.form.issueType.trim() || !this.form.programId || !this.form.policyId) return;

    // Snapshot the human-readable names from the currently-loaded dropdowns
    // so the backend can persist them on the record. This makes the record
    // self-describing even if the program / activity is later deleted.
    const program  = this.programSvc.programs().find(p => p.id === this.form.programId);
    const activity = this.form.activityId
      ? this.activitySvc.activities().find(a => a.id === this.form.activityId)
      : null;

    const payload: CreateComplianceRecordDto = {
      ...this.form,
      programName:  program?.title ?? undefined,
      activityName: activity?.title ?? null,
    };

    this.svc.createRecord(payload, () => {
      this.closeForm();
      // Jump to page 1 so the user can see the new record they just reported.
      this.page.set(1);
      this.svc.loadPagedRecords(1, this.pageSize, this.filterStatus());
    });
  }

  nextStepLabel(status: string): string {
    return status === 'Reported' ? 'Start' : status === 'InProgress' ? 'Resolve' : '';
  }

  // For Reported → InProgress: one-click. For InProgress → Closed: open the
  // resolve modal so HR captures *how* it was fixed (resolution notes).
  nextStep(record: ComplianceRecord): void {
    if (record.status === 'Reported')   this.svc.startRecord(record.id);
    if (record.status === 'InProgress') this.openResolve(record);
  }

  // ── Resolve modal (HR closes a case with resolution notes) ─────────
  resolvingRecord = signal<ComplianceRecord | null>(null);
  resolutionNotes = '';

  openResolve(r: ComplianceRecord): void {
    this.resolvingRecord.set(r);
    this.resolutionNotes = '';
  }
  cancelResolve(): void {
    this.resolvingRecord.set(null);
    this.resolutionNotes = '';
  }
  confirmResolve(): void {
    const r = this.resolvingRecord();
    if (!r || !this.resolutionNotes.trim()) return;
    this.svc.closeRecord(r.id, this.resolutionNotes.trim(), () => this.cancelResolve());
  }

  severityColor(s: string): string {
    return { Low: 'success', Medium: 'warning', High: 'danger', Critical: 'dark' }[s] ?? 'secondary';
  }

  statusColor(s: string): string {
    return { Reported: 'warning', InProgress: 'primary', Closed: 'secondary' }[s] ?? 'secondary';
  }

  private _blank(): CreateComplianceRecordDto {
    return { programId: '', activityId: null, policyId: '', issueType: '', severity: 'Medium', notes: '' };
  }
}
