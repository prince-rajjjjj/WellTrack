import { Component, Input } from '@angular/core';
import { CommonModule }     from '@angular/common';

@Component({
  selector:    'app-status-badge',
  standalone:  true,
  imports:     [CommonModule],
  templateUrl: './status-badge.html',
  styleUrl:    './status-badge.scss',
})
export class StatusBadge {
  /** Status string from backend e.g. 'Active', 'Completed', 'Pending', 'Inactive', 'Cancelled' */
  @Input() status = '';

  /** Maps status string → Bootstrap badge bg class */
  get badgeClass(): string {
    const map: Record<string, string> = {
      active:     'bg-success',
      completed:  'bg-primary',
      pending:    'bg-warning text-dark',
      inactive:   'bg-secondary',
      cancelled:  'bg-danger',
      enrolled:   'bg-info text-dark',
      inprogress: 'bg-warning text-dark',
      approved:   'bg-success',
      rejected:   'bg-danger',
    };
    return map[this.status.toLowerCase().replace(/\s+/g, '')] ?? 'bg-secondary';
  }
}
