import { Component, Input, Output, EventEmitter, OnInit, inject, signal, computed, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivityService } from '../../services/activity';
import { AttendanceDto } from '../../models/activity.model';
import { ToastService } from '../../../../shared/services/toast.service';

interface RosterRow {
  employeeID: string;
  employeeName: string;
  isPresent: boolean;
}

@Component({
  selector: 'app-attendance-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './attendance-modal.html',
})
export class AttendanceModal implements OnInit {
  @Input() sessionId!: string;       // activityId
  @Input() sessionTitle = '';
  @Input() finalized = false;        // true once coach has submitted attendance
  @Input() activityStatus: string | number = '';  // Active=Scheduled, Pending=InProgress, Completed, Cancelled
  @Output() closed = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<void>();

  private activityService = inject(ActivityService);
  private toast           = inject(ToastService);
  private destroyRef      = inject(DestroyRef);

  loading    = signal(false);
  submitting = signal(false);
  error      = signal<string | null>(null);
  roster     = signal<RosterRow[]>([]);

  presentCount = computed(() => this.roster().filter(r => r.isPresent).length);
  totalCount   = computed(() => this.roster().length);

  // Editable only when activity is in progress AND not already finalized.
  private normalizedStatus(): string {
    const s = this.activityStatus;
    if (typeof s === 'number') {
      const map: Record<number, string> = { 1:'active', 2:'inactive', 3:'pending', 4:'completed', 5:'cancelled', 6:'archived' };
      return map[s] ?? '';
    }
    return String(s ?? '').toLowerCase();
  }
  notStarted   = computed(() => this.normalizedStatus() === 'active');
  inProgress   = computed(() => this.normalizedStatus() === 'pending');
  isLocked     = computed(() => this.finalized || !this.inProgress());

  ngOnInit(): void { this.loadRoster(); }

  private loadRoster(): void {
    this.loading.set(true);
    this.activityService.getAttendance(this.sessionId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (rows: AttendanceDto[]) => {
          this.roster.set(rows.map(r => ({
            employeeID: r.employeeID,
            employeeName: r.employeeName || 'Employee',
            isPresent: r.isPresent,
          })));
          this.loading.set(false);
        },
        error: (err) => { this.error.set(err.message ?? 'Failed to load roster.'); this.loading.set(false); }
      });
  }

  toggle(row: RosterRow, present: boolean): void {
    if (this.isLocked()) return;
    this.roster.set(this.roster().map(r => r.employeeID === row.employeeID ? { ...r, isPresent: present } : r));
  }

  markAll(present: boolean): void {
    if (this.isLocked()) return;
    this.roster.set(this.roster().map(r => ({ ...r, isPresent: present })));
  }

  submit(): void {
    if (this.isLocked() || this.submitting()) return;
    if (this.roster().length === 0) { this.toast.error('No one is enrolled yet.'); return; }

    this.submitting.set(true);
    const entries = this.roster().map(r => ({ employeeID: r.employeeID, isPresent: r.isPresent }));

    this.activityService.finalizeAttendance(this.sessionId, entries)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toast.success(`Attendance submitted (${this.presentCount()}/${this.totalCount()} present).`);
          this.submitting.set(false);
          this.submitted.emit();
          this.closed.emit();
        },
        error: (err) => {
          this.error.set(err.message ?? 'Failed to submit attendance.');
          this.submitting.set(false);
        }
      });
  }

  close(): void { this.closed.emit(); }
  onBackdropClick(e: MouseEvent): void { if (e.target === e.currentTarget) this.close(); }

  initial(name: string): string { return (name?.charAt(0) || '?').toUpperCase(); }
}
