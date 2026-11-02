import {
  Component, Input, Output, EventEmitter,
  OnChanges, SimpleChanges, inject, signal,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FeedbackService } from '../../services/feedback';

@Component({
  selector: 'app-feedback-popup',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feedback-popup.html',
  styleUrl:    './feedback-popup.scss',
})
export class FeedbackPopup implements OnChanges {
  /** Set to true to display the overlay. */
  @Input() show          = false;
  /** Program the employee just completed an activity in. */
  @Input() programId     = '';
  @Input() programName   = '';
  /** The specific activity being reviewed. */
  @Input() activityId    = '';
  @Input() activityTitle = '';

  /** Emitted when the user submits or skips. */
  @Output() dismissed = new EventEmitter<void>();

  private svc = inject(FeedbackService);

  readonly stars = [1, 2, 3, 4, 5];

  rating    = signal(0);
  hoverStar = signal(0);
  comments  = signal('');
  saving    = signal(false);
  errorMsg  = signal('');
  done      = signal(false);

  // Reset form state each time the popup becomes visible
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['show']?.currentValue === true) {
      this.rating.set(0);
      this.hoverStar.set(0);
      this.comments.set('');
      this.errorMsg.set('');
      this.done.set(false);
    }
  }

  // ── Star interaction ─────────────────────────────────────────────
  activate(n: number): void { this.rating.set(n); }
  hover(n: number):    void { this.hoverStar.set(n); }
  unhover():           void { this.hoverStar.set(0); }
  isActive(n: number): boolean { return n <= (this.hoverStar() || this.rating()); }

  ratingLabel(): string {
    return ['', 'Terrible', 'Poor', 'Okay', 'Good', 'Excellent'][this.rating()] ?? '';
  }

  // ── Submit ───────────────────────────────────────────────────────
  submit(): void {
    if (this.rating() === 0) {
      this.errorMsg.set('Please pick a star rating first.');
      return;
    }
    this.saving.set(true);
    this.errorMsg.set('');

    this.svc.create({
      programId:     this.programId,
      activityId:    this.activityId   || undefined,
      activityTitle: this.activityTitle || undefined,
      rating:        this.rating(),
      comments:      this.comments().trim() || undefined,
    }).subscribe({
      next: res => {
        this.saving.set(false);
        if (res.succeeded) {
          this.done.set(true);
          setTimeout(() => this.close(), 2200);
        } else {
          this.errorMsg.set(res.message ?? 'Submission failed.');
        }
      },
      error: err => {
        this.saving.set(false);
        const msg: string = err?.error?.message ?? err?.message ?? '';
        // If already reviewed, close silently
        if (msg.toLowerCase().includes('already')) {
          this.close();
        } else {
          this.errorMsg.set(msg || 'Request failed. Please try again.');
        }
      },
    });
  }

  skip(): void { this.close(); }

  private close(): void {
    this.dismissed.emit();
  }
}
