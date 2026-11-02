import { Component, inject, signal, computed, OnInit, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { combineLatest, EMPTY } from 'rxjs';
import { catchError, debounceTime, switchMap } from 'rxjs/operators';
import { ProgramService } from '../../services/program';
import { AuthService } from '../../../../core/services/auth';
import { ToastService } from '../../../../shared/services/toast.service';
import { ProgramCard } from '../../components/program-card/program-card';
import { ProgramFormModal } from '../../components/program-form-modal/program-form-modal';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { ProgramDto } from '../../models/program.model';
import { ComplianceService } from '../../../compliance/services/compliance';

@Component({
  selector: 'app-programs',
  standalone: true,
  imports: [CommonModule, FormsModule, ProgramCard, ProgramFormModal, ConfirmModal],
  templateUrl:'./programs.html',
  styleUrl: './programs.scss',
})
export class Programs implements OnInit {
  protected programService = inject(ProgramService);
  protected complianceSvc  = inject(ComplianceService);
  private auth             = inject(AuthService);
  private toast            = inject(ToastService);
  private router           = inject(Router);
  private destroyRef       = inject(DestroyRef);

  // ── Signals ────────────────────────────────────────────────────────
  loading         = signal(false);
  searchTerm      = signal('');
  statusFilter    = signal('');
  showFormModal   = signal(false);
  showConfirm     = signal(false);
  selectedProgram = signal<ProgramDto | null>(null);
  selectedProgramId = signal<string | null>(null);

  isAdmin = this.auth.isHROrAdmin();

  /** Mirror of backend [Authorize(Roles = "Admin,HR,ComplianceOfficer")] on /compliance-records.
   *  Coach / Employee skip the call to avoid a 403 noise in the console. */
  get canReadCompliance(): boolean {
    const role = this.auth.getCurrentRole();
    return role === 'Admin' || role === 'HR' || role === 'ComplianceOfficer';
  }

  // ── Pagination (server-side) ──────────────────────────────────────
  // pagedPrograms holds the current page items returned by the server.
  // totalCount holds the server-reported total across all pages.
  page          = signal(1);
  pageSize      = 9;
  pagedPrograms = signal<ProgramDto[]>([]);
  totalCount    = signal(0);
  totalPages    = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  constructor() {
    // Filter-driven reloads pipeline (handles BOTH initial load and subsequent
    // user filter changes — single source of truth):
    //   • combineLatest emits initial signal values synchronously on subscribe;
    //     debounceTime then coalesces with any ngOnInit-driven changes into
    //     ONE initial HTTP request.
    //   • debounceTime — coalesces rapid keystrokes in the search box so we
    //     don't hammer the API on every letter.
    //   • switchMap — cancels the previous in-flight XHR when a new filter
    //     combo arrives, so out-of-order responses can't clobber the UI.
    combineLatest([
      toObservable(this.searchTerm),
      toObservable(this.statusFilter),
    ]).pipe(
      debounceTime(250),
      switchMap(([search, status]) => {
        this.page.set(1);
        this.loading.set(true);
        return this.programService.loadPagedPrograms(
          1, this.pageSize,
          search || undefined,
          status || undefined,
        ).pipe(
          catchError(err => {
            this.toast.error(err?.message ?? 'Failed to load programs.');
            this.loading.set(false);
            return EMPTY;
          })
        );
      }),
      takeUntilDestroyed(this.destroyRef),
    ).subscribe(result => {
      this.pagedPrograms.set(result.items);
      this.totalCount.set(result.totalCount);
      this.loading.set(false);
    });
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.fetchPage(p);
  }

  /** Template alias — refresh button calls this. */
  fetchPrograms(): void { this.fetchPage(this.page()); }

  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  /** Fetches a specific page from the server using the current filter state. */
  fetchPage(p: number = 1): void {
    this.page.set(p);
    this.loading.set(true);
    this.programService.loadPagedPrograms(
      p, this.pageSize,
      this.searchTerm()   || undefined,
      this.statusFilter() || undefined,
    )
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:  result => {
          this.pagedPrograms.set(result.items);
          this.totalCount.set(result.totalCount);
          this.loading.set(false);
        },
        error: (err) => { this.toast.error(err.message ?? 'Failed to load programs.'); this.loading.set(false); }
      });
  }

  // ── Computed ───────────────────────────────────────────────────────
  issuesPerProgram = computed(() => {
    const map: Record<string, number> = {};
    this.complianceSvc.records()
      .filter(r => r.status !== 'Closed')
      .forEach(r => { map[r.programId] = (map[r.programId] ?? 0) + 1; });
    return map;
  });

  openIssues = computed(() => {
    const id = this.selectedProgramId();
    return id ? this.complianceSvc.records().filter(r => r.programId === id && r.status !== 'Closed') : [];
  });

  // ── Lifecycle ──────────────────────────────────────────────────────
  ngOnInit(): void {
    // Also keep the full programs list in service cache so other consumers
    // (activities filter dropdown, compliance lookups) work even if the user
    // navigates here first. The full-list call is shared across pages.
    if (this.programService.programs().length === 0) {
      this.programService.loadPrograms()
        .pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
    }

    // No explicit fetchPage(1) here — the constructor's combineLatest pipeline
    // emits the initial filter combination and fires the first fetch
    // automatically after debounceTime.

    // Only roles that can read /compliance-records should fetch.
    if (this.canReadCompliance && this.complianceSvc.records().length === 0) {
      this.complianceSvc.loadRecords();
    }
  }

  openCreate(): void { this.selectedProgram.set(null); this.showFormModal.set(true); }
  openEdit(program: ProgramDto): void { this.selectedProgram.set(program); this.showFormModal.set(true); }

  onFormClosed(saved: boolean): void {
    const wasEdit = !!this.selectedProgram();
    this.showFormModal.set(false);
    this.selectedProgram.set(null);
    if (saved) {
      this.toast.success(wasEdit ? 'Program updated.' : 'Program created.');
      // Jump to page 1 on create, stay on current page for edit.
      this.fetchPage(wasEdit ? this.page() : 1);
      // Refresh full programs cache too, so other pages see the update.
      this.programService.loadPrograms()
        .pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
    }
  }

  confirmDelete(program: ProgramDto): void { this.selectedProgram.set(program); this.showConfirm.set(true); }

  onConfirmed(): void {
    const p = this.selectedProgram();
    if (!p) return;
    this.showConfirm.set(false);
    this.selectedProgram.set(null);
    this.programService.delete(p.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:  () => {
          this.toast.success('Program deleted.');
          this.fetchPage(this.page());
          this.programService.loadPrograms()
            .pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
        },
        error: (err) => this.toast.error(err.message ?? 'Delete failed.')
      });
  }

  // ── Compliance helpers ─────────────────────────────────────────────
  goToActivities(program: ProgramDto): void {
    this.router.navigate(['/wellness/activities'], { queryParams: { programId: program.id } });
  }

  viewIssues(programId: string): void {
    this.selectedProgramId.set(this.selectedProgramId() === programId ? null : programId);
  }

  resolveIssue(id: string, status: string): void { this.complianceSvc.resolveIssue(id, status); }

  severityColor(s: string): string {
    return ({ Low: 'success', Medium: 'warning', High: 'danger', Critical: 'dark' } as Record<string,string>)[s] ?? 'secondary';
  }
}
