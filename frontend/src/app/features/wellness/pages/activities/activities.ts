import { Component, inject, signal, computed, OnInit, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { combineLatest, EMPTY } from 'rxjs';
import { catchError, debounceTime, switchMap } from 'rxjs/operators';
import { ActivityService } from '../../services/activity';
import { ProgramService } from '../../services/program';
import { AuthService } from '../../../../core/services/auth';
import { ToastService } from '../../../../shared/services/toast.service';
import { ActivityFormModal, ActivityModalMode } from '../../components/activity-form-modal/activity-form-modal';
import { AttendanceModal } from '../../components/attendance-modal/attendance-modal';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';
import { ActivityDto } from '../../models/activity.model';
import { ParticipationService } from '../../../../features/participation/services/participation';
import { ComplianceService } from '../../../compliance/services/compliance';

// StatusType: Active=1, Inactive=2, Pending=3, Completed=4, Cancelled=5, Archived=6
// In our flow: Active=Scheduled (default), Pending=InProgress, Completed=Done, Cancelled.
function normalizeStatus(status: any): string {
  if (typeof status === 'number') {
    const map: Record<number, string> = { 1:'active', 2:'inactive', 3:'pending', 4:'completed', 5:'cancelled', 6:'archived' };
    return map[status] ?? '';
  }
  return String(status ?? '').toLowerCase();
}

type ConfirmAction = 'complete' | 'cancel' | 'start';

@Component({
  selector: 'app-activities',
  standalone: true,
  imports: [CommonModule, FormsModule, ActivityFormModal, AttendanceModal, ConfirmModal, Skeleton],
  templateUrl: './activities.html',
  styleUrl: './activities.scss',
})
export class Activities implements OnInit {
  protected activityService  = inject(ActivityService);
  protected programService   = inject(ProgramService);
  private auth               = inject(AuthService);
  private toast              = inject(ToastService);
  private router             = inject(Router);
  private route              = inject(ActivatedRoute);
  private destroyRef         = inject(DestroyRef);
  private participationSvc   = inject(ParticipationService);
  protected complianceSvc    = inject(ComplianceService);

  issuesPerActivity = computed(() => {
    const map: Record<string, number> = {};
    this.complianceSvc.records()
      .filter(r => r.status !== 'Closed' && r.activityId)
      .forEach(r => { map[r.activityId!] = (map[r.activityId!] ?? 0) + 1; });
    return map;
  });

  loading          = signal(false);
  searchTerm       = signal('');
  programFilter    = signal('');
  statusFilter     = signal('');
  modeFilter       = signal('');
  viewMode         = signal<'all' | 'today' | 'upcoming' | 'completed' | 'cancelled'>('today');

  // ── Pagination (server-side) ──────────────────────────────────────
  // pagedActivities holds the current page items returned by the server.
  // totalCount holds the server-reported total across all pages.
  page             = signal(1);
  pageSize         = 9;
  pagedActivities  = signal<ActivityDto[]>([]);
  totalCount       = signal(0);
  totalPages       = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  showModal        = signal(false);
  showAttendance   = signal(false);
  showConfirm      = signal(false);
  modalMode        = signal<ActivityModalMode>('create');
  confirmAction    = signal<ConfirmAction>('complete');
  selectedActivity = signal<ActivityDto | null>(null);

  isAdmin    = this.auth.isHROrAdmin();
  isCoach    = this.auth.isCoach();
  isEmployee = this.auth.isEmployee();
  currentUserId = this.auth.getCurrentUserId();

  filteredActivities = computed(() => {
    let list = this.activityService.activities();
    const term    = this.searchTerm().toLowerCase().trim();
    const program = this.programFilter();
    const status  = this.statusFilter().toLowerCase();
    const mode    = this.modeFilter();
    const view    = this.viewMode();

    // "Today" boundaries in local time
    const startOfToday = new Date(); startOfToday.setHours(0, 0, 0, 0);
    const startOfTomorrow = new Date(startOfToday); startOfTomorrow.setDate(startOfTomorrow.getDate() + 1);

    // Coach sees only their assigned activities
    if (this.isCoach) list = list.filter(a => a.assignedCoachID === this.currentUserId);

    if (view !== 'all') {
      list = list.filter(a => {
        const s = normalizeStatus(a.status);
        const d = a.date ? new Date(a.date) : null;
        const isCancelled = s === 'cancelled';
        const isCompleted = !isCancelled && (s === 'completed' || (!!d && d < startOfToday));
        const isToday     = !isCancelled && !isCompleted && !!d && d >= startOfToday && d < startOfTomorrow;
        const isUpcoming  = !isCancelled && !isCompleted && !!d && d >= startOfTomorrow;

        switch (view) {
          case 'today':     return isToday;
          case 'upcoming':  return isUpcoming;
          case 'completed': return isCompleted;
          case 'cancelled': return isCancelled;
        }
      });
    }

    if (term)    list = list.filter(a => a.title.toLowerCase().includes(term) || a.type.toLowerCase().includes(term));
    if (program) list = list.filter(a => a.programID === program);
    if (status)  list = list.filter(a => normalizeStatus(a.status) === status);
    if (mode)    list = list.filter(a => String(a.mode).toLowerCase() === mode.toLowerCase());
    return list;
  });

  constructor() {
    // Auto-refetch whenever any filter (including tab) changes.
    // combineLatest emits the initial values on subscribe, so this also
    // performs the first load — but ngOnInit's query-param sync runs first,
    // so the URL's ?programId= is already applied by the time we fire.
    combineLatest([
      toObservable(this.searchTerm),
      toObservable(this.programFilter),
      toObservable(this.statusFilter),
      toObservable(this.modeFilter),
      toObservable(this.viewMode),
    ]).pipe(
      debounceTime(200),                       // coalesce rapid typing
      switchMap(() => {
        this.page.set(1);                      // any filter change → reset to page 1
        this.loading.set(true);
        return this.activityService.loadPagedActivities(
          1, this.pageSize,
          this.programFilter() || undefined,
          this.searchTerm()    || undefined,
          this.modeFilter()    || undefined,
          this.statusFilter()  || undefined,
          this.viewMode(),
        ).pipe(catchError(err => {
          this.toast.error(err?.message ?? 'Failed to load activities.');
          this.loading.set(false);
          return EMPTY;
        }));
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(result => {
      this.pagedActivities.set(result.items);
      this.totalCount.set(result.totalCount);
      this.loading.set(false);
    });
  }

  ngOnInit(): void {
    // Pre-fill program filter from ?programId=... and switch to "All" so the
    // user can see every activity inside that program at a glance.
    // The constructor's pipeline picks up these signal changes and fetches.
    const programId = this.route.snapshot.queryParamMap.get('programId');
    if (programId) {
      this.programFilter.set(programId);
      this.viewMode.set('all');
    }
    // No explicit fetchAll() — the combineLatest pipeline above fires the initial load.
  }

  /** Mirror of the backend's [Authorize(Roles = "Admin,HR,ComplianceOfficer")]. */
  get canReadCompliance(): boolean {
    const role = this.auth.getCurrentRole();
    return role === 'Admin' || role === 'HR' || role === 'ComplianceOfficer';
  }

  /** Backwards-compat name for the refresh button — re-fetches the current page. */
  fetchAll(): void {
    this.fetchPage(this.page());
  }

  /** Server-side paginated fetch — populates pagedActivities + totalCount. */
  fetchPage(p: number = 1): void {
    this.page.set(p);
    this.loading.set(true);
    this.activityService.loadPagedActivities(
      p, this.pageSize,
      this.programFilter() || undefined,
      this.searchTerm()    || undefined,
      this.modeFilter()    || undefined,
      this.statusFilter()  || undefined,
      this.viewMode(),
    )
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: result => {
          this.pagedActivities.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: err => {
          this.toast.error(err.message ?? 'Failed to load activities.');
          this.loading.set(false);
        }
      });
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.fetchPage(p);
  }

  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  // ─── Create / Assign ────────────────────────────────────
  openCreate(): void {
    this.modalMode.set('create');
    this.selectedActivity.set(null);
    this.showModal.set(true);
  }

  openEdit(a: ActivityDto): void {
    this.modalMode.set('edit');
    this.selectedActivity.set(a);
    this.showModal.set(true);
  }

  openAssignCoach(a: ActivityDto): void {
    this.modalMode.set('assign-coach');
    this.selectedActivity.set(a);
    this.showModal.set(true);
  }

  // Editable while still scheduled (Active). Once started, locked.
  canEdit(a: ActivityDto): boolean {
    return this.isAdmin && normalizeStatus(a.status) === 'active';
  }

  onModalClosed(saved: boolean): void {
    const mode = this.modalMode();
    this.showModal.set(false);
    this.selectedActivity.set(null);
    if (saved) {
      const msg = mode === 'create' ? 'Activity created.'
                : mode === 'edit'   ? 'Activity updated.'
                                    : 'Coach assigned.';
      this.toast.success(msg);
      // After create/edit, jump to page 1 so the user sees their change immediately.
      this.fetchPage(mode === 'create' ? 1 : this.page());
    }
  }

  // ─── Lifecycle actions ──────────────────────────────────
  startActivity(a: ActivityDto): void {
    this.selectedActivity.set(a);
    this.confirmAction.set('start');
    this.showConfirm.set(true);
  }

  confirmComplete(a: ActivityDto): void {
    this.selectedActivity.set(a);
    this.confirmAction.set('complete');
    this.showConfirm.set(true);
  }

  confirmCancel(a: ActivityDto): void {
    this.selectedActivity.set(a);
    this.confirmAction.set('cancel');
    this.showConfirm.set(true);
  }

  onConfirmed(): void {
    const a = this.selectedActivity();
    if (!a) return;
    const action = this.confirmAction();
    this.showConfirm.set(false);

    const op$ =
      action === 'start'    ? this.activityService.start(a.id) :
      action === 'cancel'   ? this.activityService.cancel(a.id) :
                              this.activityService.complete(a.id);

    op$.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:  () => {
          this.toast.success(this.successMessage(action));
          this.fetchPage(this.page());
        },
        error: (err) => {
          this.toast.error(err.message ?? 'Action failed.');
          // Whoever lost the race had stale state — refresh so they see reality.
          this.fetchPage(this.page());
        }
      });
  }

  private successMessage(a: ConfirmAction): string {
    if (a === 'start')    return 'Activity started.';
    if (a === 'cancel')   return 'Activity cancelled.';
    return 'Activity marked complete.';
  }

  // ─── Employee actions ───────────────────────────────────
  joinActivity(a: ActivityDto): void {
    this.activityService.join(a.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toast.success('Joined activity.');
          this.participationSvc.joinActivity(a.programID, a.id, a.title);
          if (a.mode === 'Online') {
            this.router.navigate(['/wellness/activities', a.id]);
          } else {
            this.fetchPage(this.page());
          }
        },
        error: (err) => {
          this.toast.error(err.message ?? 'Failed to join.');
          this.fetchPage(this.page());
        }
      });
  }

  leaveActivity(a: ActivityDto): void {
    this.activityService.leave(a.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:  () => { this.toast.success('Left activity.'); this.fetchPage(this.page()); },
        error: (err) => this.toast.error(err.message ?? 'Failed to leave.')
      });
  }

  goToDetails(a: ActivityDto): void {
    this.router.navigate(['/wellness/activities', a.id]);
  }

  // Employee acts on Online: join (if not yet) → details (with iframe + Mark Complete)
  watchOrJoin(a: ActivityDto): void {
    if (!a.isEnrolled) {
      this.activityService.join(a.id)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe({
          next: () => {
            this.participationSvc.joinActivity(a.programID, a.id, a.title);
            this.router.navigate(['/wellness/activities', a.id]);
          },
          error: (err) => this.toast.error(err.message ?? 'Failed to join.')
        });
    } else {
      this.goToDetails(a);
    }
  }

  // ─── Coach modals ───────────────────────────────────────
  openAttendance(a: ActivityDto): void {
    this.selectedActivity.set(a);
    this.showAttendance.set(true);
  }

  // ─── Display helpers ────────────────────────────────────
  typeIcon(type: string): string {
    const map: Record<string, string> = {
      yoga: 'bi-peace', running: 'bi-person-walking', workshop: 'bi-people-fill',
      meditation: 'bi-moon-stars-fill', hiit: 'bi-lightning-fill',
      cycling: 'bi-bicycle', swimming: 'bi-water', nutrition: 'bi-egg-fried',
    };
    return map[type?.toLowerCase()] ?? 'bi-activity';
  }

  statusClass(status: any): string {
    switch (normalizeStatus(status)) {
      case 'active':    return 'bg-success-subtle text-success';
      case 'pending':   return 'bg-warning-subtle text-warning';
      case 'completed': return 'bg-primary-subtle text-primary';
      case 'cancelled': return 'bg-danger-subtle text-danger';
      case 'inactive':  return 'bg-secondary-subtle text-secondary';
      default:          return 'bg-secondary-subtle text-secondary';
    }
  }

  statusLabel(status: any): string {
    const map: Record<string, string> = {
      active: 'Scheduled', pending: 'In Progress', completed: 'Completed',
      cancelled: 'Cancelled', inactive: 'Inactive',
    };
    const s = normalizeStatus(status);
    return map[s] ?? (s.charAt(0).toUpperCase() + s.slice(1));
  }

  modeBadge(mode: any): { cls: string; icon: string; label: string } {
    const m = String(mode ?? '').toLowerCase();
    if (m === 'online') return { cls: 'bg-info-subtle text-info', icon: 'bi-play-btn',  label: 'Online' };
    return { cls: 'bg-success-subtle text-success', icon: 'bi-geo-alt', label: 'Offline' };
  }

  // ─── Status guards ──────────────────────────────────────
  isCancelled(a: ActivityDto): boolean { return normalizeStatus(a.status) === 'cancelled'; }
  isActivityCompleted(a: ActivityDto): boolean { return normalizeStatus(a.status) === 'completed'; }
  canJoin(a: ActivityDto): boolean {
    const s = normalizeStatus(a.status);
    return this.isEmployee && !a.isEnrolled && s !== 'cancelled' && s !== 'completed';
  }

  // ─── Coach guards ───────────────────────────────────────
  isMyActivity(a: ActivityDto): boolean { return this.isCoach && a.assignedCoachID === this.currentUserId; }
  canStart(a: ActivityDto): boolean    { return this.isMyActivity(a) && normalizeStatus(a.status) === 'active'; }
  canComplete(a: ActivityDto): boolean { return this.isMyActivity(a) && (normalizeStatus(a.status) === 'pending' || normalizeStatus(a.status) === 'active'); }
  canCancel(a: ActivityDto): boolean   {
    const s = normalizeStatus(a.status);
    return this.isAdmin && s !== 'completed' && s !== 'cancelled';
  }

  capacityPct(a: ActivityDto): number {
    if (!a.maxParticipants) return 0;
    return Math.min(100, Math.round((a.enrolledCount / a.maxParticipants) * 100));
  }

  capacityColor(pct: number): string {
    if (pct >= 90) return 'bg-danger';
    if (pct >= 60) return 'bg-warning';
    return 'bg-success';
  }
}