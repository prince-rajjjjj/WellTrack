import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedbackService } from '../../services/feedback';
import { FeedbackDto } from '../../models/feedback.model';
import { FeedbackForm } from '../../components/feedback-form/feedback-form';
import { ProgramService } from '../../../wellness/services/program';
import { ActivityService } from '../../../wellness/services/activity';
import { ActivityDto } from '../../../wellness/models/activity.model';
import { UserAdminService } from '../../../identity/services/user-admin';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-feedback',
  standalone: true,
  imports: [CommonModule, FormsModule, FeedbackForm],
  templateUrl: './feedback.html',
  styleUrl: './feedback.scss',
})
export class Feedback implements OnInit {
  private svc         = inject(FeedbackService);
  private programSvc  = inject(ProgramService);
  private activitySvc = inject(ActivityService);
  private userSvc     = inject(UserAdminService);
  private auth        = inject(AuthService);

  // ── Role flags ────────────────────────────────────────────────────
  readonly isHR    = this.auth.isHROrAdmin();
  readonly isCoach = this.auth.isCoach();
  readonly isEmp   = this.auth.isEmployee();

  // ── Shared state ──────────────────────────────────────────────────
  loading = signal(false);
  error   = signal('');

  // ── Employee view ─────────────────────────────────────────────────
  myFeedbacks       = signal<FeedbackDto[]>([]);
  attendedActivities = signal<ActivityDto[]>([]);
  showNewForm       = signal(false);
  newActivityId     = signal('');
  editingId         = signal<string | null>(null);
  deletingId        = signal<string | null>(null);

  // ── Coach / HR view ───────────────────────────────────────────────
  allFeedbacks    = signal<FeedbackDto[]>([]);
  selectedProgram = signal('');   // filter

  // ── Pagination ────────────────────────────────────────────────────
  // Employee cards: 6 per page (2-column grid)
  empFbPage     = signal(1);
  empFbPageSize = 6;
  empFbTotal    = computed(() => this.myFeedbacks().length);
  empFbPages    = computed(() => Math.max(1, Math.ceil(this.empFbTotal() / this.empFbPageSize)));
  pagedMyFeedbacks = computed(() => {
    const start = (this.empFbPage() - 1) * this.empFbPageSize;
    return this.myFeedbacks().slice(start, start + this.empFbPageSize);
  });
  empFbGoToPage(p: number): void { if (p >= 1 && p <= this.empFbPages()) this.empFbPage.set(p); }
  empFbPageNumbers(): number[] {
    const t = this.empFbPages(), c = this.empFbPage(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  // Coach/HR groups: 5 groups per page
  grpPage     = signal(1);
  grpPageSize = 5;
  grpTotal    = computed(() => this.programGroups().length);
  grpPages    = computed(() => Math.max(1, Math.ceil(this.grpTotal() / this.grpPageSize)));
  pagedGroups = computed(() => {
    const start = (this.grpPage() - 1) * this.grpPageSize;
    return this.programGroups().slice(start, start + this.grpPageSize);
  });
  grpGoToPage(p: number): void { if (p >= 1 && p <= this.grpPages()) this.grpPage.set(p); }
  grpPageNumbers(): number[] {
    const t = this.grpPages(), c = this.grpPage(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  // ── Name caches ───────────────────────────────────────────────────
  private programMap: Record<string, string> = {};
  private userMap:    Record<string, string> = {};

  readonly stars = [1, 2, 3, 4, 5];

  // ── Computed: org-wide stats (HR) ─────────────────────────────────
  orgStats = computed(() => {
    const fbs = this.allFeedbacks();
    if (!fbs.length) return null;
    const total    = fbs.length;
    const avgRaw   = fbs.reduce((s, f) => s + f.rating, 0) / total;
    const fiveStar = fbs.filter(f => f.rating === 5).length;
    return {
      total,
      avgRating:    +avgRaw.toFixed(1),
      fiveStarCount: fiveStar,
      fiveStarPct:  Math.round(fiveStar / total * 100),
      programCount: new Set(fbs.map(f => f.programID)).size,
    };
  });

  // ── Attended activities the employee hasn't reviewed yet ──────────
  availableActivities = computed(() => {
    const reviewed = new Set(
      this.myFeedbacks()
        .filter(f => f.activityID)
        .map(f => f.activityID!)
    );
    return this.attendedActivities().filter(a => !reviewed.has(a.id));
  });

  // ── Filtered list for coach / HR table ───────────────────────────
  filteredFeedbacks = computed(() => {
    const pid = this.selectedProgram();
    return pid
      ? this.allFeedbacks().filter(f => f.programID === pid)
      : this.allFeedbacks();
  });

  // ── Unique program IDs in feedback (for filter dropdown) ─────────
  knownPrograms = computed(() =>
    [...new Set(this.allFeedbacks().map(f => f.programID))]
  );

  // ── Grouped view for coach / HR (one section per program) ────────
  programGroups = computed(() => {
    const sel = this.selectedProgram();
    return this.knownPrograms()
      .filter(pid => !sel || pid === sel)
      .map(pid => {
        const fbs     = this.allFeedbacks().filter(f => f.programID === pid);
        const avgRaw  = fbs.reduce((s, f) => s + f.rating, 0) / fbs.length;
        return {
          programId:   pid,
          name:        this.programMap[pid] ?? `Program · ${pid.slice(0, 8).toUpperCase()}`,
          count:       fbs.length,
          avgRating:   +avgRaw.toFixed(1),
          avgRounded:  Math.round(avgRaw),
          feedbacks:   fbs.slice().sort((a, b) =>
            new Date(b.date).getTime() - new Date(a.date).getTime()),
        };
      })
      .sort((a, b) => b.count - a.count);
  });

  // ── Lifecycle ─────────────────────────────────────────────────────
  ngOnInit(): void {
    if (this.isHR)         this.loadHrView();
    else if (this.isCoach) this.loadCoachView();
    else                   this.loadEmployeeView();
  }

  // ── Loaders ───────────────────────────────────────────────────────

  private loadEmployeeView(): void {
    this.loading.set(true);

    // Load activities to find attended ones (for the "new feedback" selector)
    this.activitySvc.loadActivities().subscribe({
      next: acts => {
        this.attendedActivities.set(acts.filter(a => a.hasAttended));
        this.buildProgramMap();
      },
    });

    this.svc.getMyFeedback().subscribe({
      next: res => {
        this.loading.set(false);
        if (res.succeeded) this.myFeedbacks.set(res.data ?? []);
        else this.error.set(res.message ?? 'Failed to load feedback.');
      },
      error: () => { this.loading.set(false); this.error.set('Request failed.'); },
    });
  }

  private loadCoachView(): void {
    this.loading.set(true);
    const myId = this.auth.getCurrentUserId();

    this.activitySvc.loadActivities().subscribe({
      next: activities => {
        const mine       = activities.filter(a => a.assignedCoachID === myId);
        const programIds = [...new Set(mine.map(a => a.programID))];

        mine.forEach(a => { this.programMap[a.programID] = a.programName; });

        if (programIds.length === 0) { this.loading.set(false); return; }

        let pending = programIds.length;
        const collected: FeedbackDto[] = [];

        programIds.forEach(pid => {
          this.svc.getByProgram(pid).subscribe({
            next: res => {
              if (res.succeeded && res.data) collected.push(...res.data);
              if (--pending === 0) {
                this.allFeedbacks.set(
                  collected.sort((a, b) =>
                    new Date(b.date).getTime() - new Date(a.date).getTime())
                );
                this.loading.set(false);
              }
            },
            error: () => { if (--pending === 0) this.loading.set(false); },
          });
        });
      },
      error: () => {
        this.loading.set(false);
        this.error.set('Failed to load your assigned activities.');
      },
    });
  }

  private loadHrView(): void {
    this.loading.set(true);

    if (this.programSvc.programs().length === 0) {
      this.programSvc.loadPrograms().subscribe({ next: () => this.buildProgramMap() });
    } else {
      this.buildProgramMap();
    }

    if (this.userSvc.users().length === 0) {
      this.userSvc.loadEmployees().subscribe({
        next: () => { for (const u of this.userSvc.users()) this.userMap[u.id] = u.name; },
      });
    } else {
      for (const u of this.userSvc.users()) this.userMap[u.id] = u.name;
    }

    this.svc.getAllFeedback().subscribe({
      next: res => {
        this.loading.set(false);
        if (res.succeeded) this.allFeedbacks.set(res.data ?? []);
        else this.error.set(res.message ?? 'Failed to load feedback.');
      },
      error: () => { this.loading.set(false); this.error.set('Request failed.'); },
    });
  }

  private buildProgramMap(): void {
    for (const p of this.programSvc.programs()) this.programMap[p.id] = p.title;
  }

  // ── Employee actions ──────────────────────────────────────────────

  openNewForm(): void {
    this.newActivityId.set(this.availableActivities()[0]?.id ?? '');
    this.showNewForm.set(true);
  }

  /** Convenience: get the full ActivityDto for the currently-selected activity */
  get selectedActivity(): ActivityDto | undefined {
    return this.availableActivities().find(a => a.id === this.newActivityId());
  }

  closeNewForm(): void { this.showNewForm.set(false); }

  setSelectedProgram(v: string): void { this.selectedProgram.set(v); this.grpPage.set(1); }

  onNewSubmitted(): void {
    this.showNewForm.set(false);
    this.loadEmployeeView();
  }

  startEdit(id: string): void { this.editingId.set(id); }
  cancelEdit(): void           { this.editingId.set(null); }

  onUpdated(): void {
    this.editingId.set(null);
    this.loadEmployeeView();
  }

  confirmDelete(fb: FeedbackDto): void {
    if (!confirm('Delete this feedback? This cannot be undone.')) return;
    this.deletingId.set(fb.id);
    this.svc.delete(fb.id).subscribe({
      next: res => {
        this.deletingId.set(null);
        if (res.succeeded) this.myFeedbacks.update(l => l.filter(f => f.id !== fb.id));
      },
      error: () => this.deletingId.set(null),
    });
  }

  // ── Helpers ───────────────────────────────────────────────────────

  programName(id: string): string {
    return this.programMap[id] ?? `Program · ${id.slice(0, 8).toUpperCase()}`;
  }

  employeeName(id: string): string {
    return this.userMap[id] ?? `Emp · ${id.slice(0, 8).toUpperCase()}`;
  }

  employeeInitial(id: string): string {
    return this.employeeName(id).charAt(0).toUpperCase();
  }

  starFilled(rating: number, n: number): boolean { return n <= rating; }

  formatDate(d: string): string {
    return new Date(d).toLocaleDateString('en-US', {
      year: 'numeric', month: 'short', day: 'numeric',
    });
  }

  ratingLabel(r: number): string {
    return ['', 'Terrible', 'Poor', 'Okay', 'Good', 'Excellent'][r] ?? '';
  }

  ratingColorClass(r: number): string {
    if (r >= 4) return 'text-success';
    if (r === 3) return 'text-warning';
    return 'text-danger';
  }

  /** Returns [1, 2, … n] — use with @for in templates */
  range(n: number): number[] {
    return Array.from({ length: n }, (_, i) => i + 1);
  }
}
