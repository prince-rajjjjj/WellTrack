import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnInit,
  inject,
  signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedbackService } from '../../services/feedback';
import {
  FeedbackDto,
  CreateFeedbackDto,
  UpdateFeedbackDto,
} from '../../models/feedback.model';

@Component({
  selector: 'app-feedback-form',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback-form.html',
  styleUrl: './feedback-form.scss',
})
export class FeedbackForm implements OnInit {
  /** Required when submitting new feedback */
  @Input() programId!: string;
  /** Pass both to submit activity-level feedback */
  @Input() activityId?: string;
  @Input() activityTitle?: string;
  /** Provide this to switch the form into edit mode */
  @Input() existingFeedback?: FeedbackDto;

  @Output() submitted  = new EventEmitter<void>();
  @Output() cancelled  = new EventEmitter<void>();

  private svc = inject(FeedbackService);

  readonly stars = [1, 2, 3, 4, 5];

  // ── Signals ───────────────────────────────────────────────────────
  rating      = signal(0);
  hoverRating = signal(0);
  comments    = signal('');
  saving      = signal(false);
  error       = signal('');
  /** True while showing the "Are you sure?" confirmation step */
  confirming  = signal(false);

  get isEdit(): boolean { return !!this.existingFeedback; }

  ngOnInit(): void {
    if (this.existingFeedback) {
      this.rating.set(this.existingFeedback.rating);
      this.comments.set(this.existingFeedback.comments ?? '');
    }
  }

  // ── Star interactions ─────────────────────────────────────────────
  setRating(n: number): void { this.rating.set(n); }
  setHover(n: number): void  { this.hoverRating.set(n); }
  clearHover(): void          { this.hoverRating.set(0); }

  isStarActive(n: number): boolean {
    return n <= (this.hoverRating() || this.rating());
  }

  // ── Submit (two-step: validate → confirm → post) ─────────────────
  requestConfirm(): void {
    if (this.rating() === 0) {
      this.error.set('Please select a star rating before submitting.');
      return;
    }
    this.error.set('');
    this.confirming.set(true);
  }

  backToForm(): void { this.confirming.set(false); }

  submit(): void {
    this.saving.set(true);
    this.error.set('');

    if (this.isEdit) {
      const dto: UpdateFeedbackDto = {
        rating:   this.rating(),
        comments: this.comments().trim() || null,
      };
      this.svc.update(this.existingFeedback!.id, dto).subscribe({
        next: (res) => {
          this.saving.set(false);
          if (res.succeeded) { this.confirming.set(false); this.submitted.emit(); }
          else { this.confirming.set(false); this.error.set(res.message ?? 'Update failed.'); }
        },
        error: () => { this.saving.set(false); this.confirming.set(false); this.error.set('Request failed. Please try again.'); },
      });
    } else {
      const dto: CreateFeedbackDto = {
        programId:     this.programId,
        activityId:    this.activityId,
        activityTitle: this.activityTitle,
        rating:        this.rating(),
        comments:      this.comments().trim() || undefined,
      };
      this.svc.create(dto).subscribe({
        next: (res) => {
          this.saving.set(false);
          if (res.succeeded) { this.confirming.set(false); this.submitted.emit(); }
          else { this.confirming.set(false); this.error.set(res.message ?? 'Submission failed.'); }
        },
        error: () => { this.saving.set(false); this.confirming.set(false); this.error.set('Request failed. Please try again.'); },
      });
    }
  }

  cancel(): void { this.confirming.set(false); this.cancelled.emit(); }
}
