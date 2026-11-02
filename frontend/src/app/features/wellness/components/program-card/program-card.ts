import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProgramDto } from '../../models/program.model';

// Backend StatusType enum: Active=1, Inactive=2, Pending=3, Completed=4, Cancelled=5, Archived=6
const STATUS_MAP: Record<number, string> = {
  1: 'active', 2: 'inactive', 3: 'pending', 4: 'completed', 5: 'cancelled', 6: 'archived'
};

function normalizeStatus(status: any): string {
  if (typeof status === 'number') return STATUS_MAP[status] ?? 'unknown';
  return String(status ?? '').toLowerCase();
}

@Component({
  selector: 'app-program-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './program-card.html',
  styleUrl: './program-card.scss',
})
export class ProgramCard {
  @Input() program!: ProgramDto;
  @Input() isAdmin    = false;
  @Input() issueCount = 0;

  @Output() edit       = new EventEmitter<ProgramDto>();
  @Output() delete     = new EventEmitter<ProgramDto>();
  @Output() issueClick = new EventEmitter<void>();

  statusLabel(status: any): string {
    const s = normalizeStatus(status);
    return s.charAt(0).toUpperCase() + s.slice(1);
  }

  statusClass(status: any): string {
    switch (normalizeStatus(status)) {
      case 'active':    return 'bg-success-subtle text-success';
      case 'completed': return 'bg-primary-subtle text-primary';
      case 'pending':   return 'bg-warning-subtle text-warning';
      case 'inactive':  return 'bg-secondary-subtle text-secondary';
      case 'cancelled': return 'bg-danger-subtle text-danger';
      case 'archived':  return 'bg-dark-subtle text-dark';
      default:          return 'bg-secondary-subtle text-secondary';
    }
  }

  statusIcon(status: any): string {
    switch (normalizeStatus(status)) {
      case 'active':    return 'bi-check-circle-fill';
      case 'completed': return 'bi-trophy-fill';
      case 'pending':   return 'bi-clock-fill';
      case 'inactive':  return 'bi-pause-circle-fill';
      case 'cancelled': return 'bi-x-circle-fill';
      default:          return 'bi-circle-fill';
    }
  }

}
