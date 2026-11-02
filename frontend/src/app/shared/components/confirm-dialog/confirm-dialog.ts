import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Reusable "are you sure?" confirmation dialog.
 *
 * Usage:
 *   <app-confirm-dialog
 *     [open]="showDelete"
 *     title="Delete Policy"
 *     message="Are you sure you want to permanently delete this policy?"
 *     confirmLabel="Delete"
 *     tone="danger"
 *     (confirm)="onDelete()"
 *     (cancel)="showDelete = false">
 *   </app-confirm-dialog>
 *
 * Plain Bootstrap utilities — no custom complex CSS.
 */
@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (open) {
      <div class="modal-backdrop-wt" (click)="cancel.emit()">
        <div class="card border-0 shadow-lg rounded-4 p-4 confirm-card"
             (click)="$event.stopPropagation()">
          <div class="d-flex align-items-start gap-3 mb-3">
            <div class="confirm-icon" [class]="'bg-' + tone + '-subtle text-' + tone">
              <i class="bi" [class]="iconClass"></i>
            </div>
            <div class="flex-grow-1">
              <h6 class="fw-bold mb-1">{{ title }}</h6>
              <p class="text-muted small mb-0">{{ message }}</p>
            </div>
          </div>

          <div class="d-flex justify-content-end gap-2 mt-2">
            <button type="button" class="btn btn-light btn-sm rounded-3 px-3"
                    (click)="cancel.emit()">
              {{ cancelLabel }}
            </button>
            <button type="button" class="btn btn-sm rounded-3 px-3"
                    [class]="'btn-' + tone"
                    (click)="confirm.emit()">
              {{ confirmLabel }}
            </button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .modal-backdrop-wt {
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1080;
      animation: fade-in 0.12s ease-out;
    }
    .confirm-card { max-width: 420px; width: 92%; }
    .confirm-icon {
      width: 40px;
      height: 40px;
      border-radius: 10px;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-shrink: 0;
      font-size: 1.1rem;
    }
    @keyframes fade-in {
      from { opacity: 0; }
      to   { opacity: 1; }
    }
  `],
})
export class ConfirmDialog {
  /** Controls visibility — bind a boolean signal/property. */
  @Input() open = false;
  @Input() title = 'Are you sure?';
  @Input() message = 'This action cannot be undone.';
  @Input() confirmLabel = 'Confirm';
  @Input() cancelLabel = 'Cancel';
  /** Bootstrap tone — drives icon color + confirm button color. */
  @Input() tone: 'danger' | 'warning' | 'success' | 'primary' = 'danger';

  @Output() confirm = new EventEmitter<void>();
  @Output() cancel  = new EventEmitter<void>();

  /** Map tone → icon. Kept inline so the host doesn't need to think about it. */
  get iconClass(): string {
    return {
      danger:  'bi-exclamation-triangle-fill',
      warning: 'bi-exclamation-circle-fill',
      success: 'bi-check-circle-fill',
      primary: 'bi-question-circle-fill',
    }[this.tone];
  }
}
