import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable confirmation dialog.
 *
 * Usage:
 *   <app-confirm-modal
 *     [visible]="showConfirm"
 *     title="Delete Goal"
 *     message="Are you sure you want to delete this goal? This cannot be undone."
 *     confirmLabel="Delete"
 *     confirmClass="btn-danger"
 *     (confirmed)="onDelete()"
 *     (cancelled)="showConfirm = false">
 *   </app-confirm-modal>
 */
@Component({
  selector:    'app-confirm-modal',
  standalone:  true,
  imports:     [CommonModule],
  templateUrl: './confirm-modal.html',
  styleUrl:    './confirm-modal.scss',
})
export class ConfirmModal {
  @Input() visible       = false;
  @Input() title         = 'Confirm Action';
  @Input() message       = 'Are you sure you want to proceed?';
  @Input() confirmLabel  = 'Confirm';
  @Input() cancelLabel   = 'Cancel';
  @Input() confirmClass  = 'btn-primary';

  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  onConfirm(): void {
    this.confirmed.emit();
  }

  onCancel(): void {
    this.cancelled.emit();
  }
}
