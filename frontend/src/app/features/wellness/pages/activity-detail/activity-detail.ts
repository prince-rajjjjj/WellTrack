import { Component, inject, signal, computed, OnInit, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivityService } from '../../services/activity';
import { AuthService } from '../../../../core/services/auth';
import { ToastService } from '../../../../shared/services/toast.service';
import { ActivityDto } from '../../models/activity.model';
import { FeedbackService } from '../../../engagement/services/feedback';
import { FeedbackPopup } from '../../../engagement/components/feedback-popup/feedback-popup';

function normalizeStatus(status: any): string {
  if (typeof status === 'number') {
    const map: Record<number, string> = { 1:'active', 2:'inactive', 3:'pending', 4:'completed', 5:'cancelled', 6:'archived' };
    return map[status] ?? '';
  }
  return String(status ?? '').toLowerCase();
}

// Convert any common YouTube URL form into the embed form so it plays in-page (no redirect).
function toEmbedUrl(url?: string | null): string | null {
  if (!url) return null;
  try {
    const u = new URL(url);
    // youtu.be/<id>
    if (u.hostname.includes('youtu.be')) {
      const id = u.pathname.replace(/^\//, '');
      return id ? `https://www.youtube.com/embed/${id}` : null;
    }
    // youtube.com/watch?v=<id>
    if (u.hostname.includes('youtube.com')) {
      if (u.pathname === '/watch') {
        const id = u.searchParams.get('v');
        return id ? `https://www.youtube.com/embed/${id}` : null;
      }
      // Already embed form
      if (u.pathname.startsWith('/embed/')) return url;
    }
  } catch { /* fall through */ }
  return url; // hand whatever we got, the iframe will fail loudly if bad
}

@Component({
  selector: 'app-activity-detail',
  standalone: true,
  imports: [CommonModule, FeedbackPopup],
  templateUrl: './activity-detail.html',
  styleUrl: './activity-detail.scss',
})
export class ActivityDetail implements OnInit {
  private route           = inject(ActivatedRoute);
  private router          = inject(Router);
  private activityService = inject(ActivityService);
  private auth            = inject(AuthService);
  private toast           = inject(ToastService);
  private sanitizer       = inject(DomSanitizer);
  private feedbackSvc     = inject(FeedbackService);
  private destroyRef      = inject(DestroyRef);

  loading  = signal(true);
  error    = signal<string | null>(null);
  activity = signal<ActivityDto | null>(null);

  isAdmin    = this.auth.isHROrAdmin();
  isCoach    = this.auth.isCoach();
  isEmployee = this.auth.isEmployee();

  // ── Feedback popup state ─────────────────────────────────────────
  showFeedbackPopup = signal(false);

  embedUrl = computed<SafeResourceUrl | null>(() => {
    const a = this.activity();
    if (!a || a.mode !== 'Online') return null;
    const url = toEmbedUrl(a.videoUrl);
    return url ? this.sanitizer.bypassSecurityTrustResourceUrl(url) : null;
  });

  status = computed(() => normalizeStatus(this.activity()?.status));
  isCancelled = computed(() => this.status() === 'cancelled');
  isCompleted = computed(() => this.status() === 'completed');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) { this.router.navigate(['/wellness/activities']); return; }
    this.fetch(id);
  }

  private fetch(id: string): void {
    this.loading.set(true);
    this.activityService.getById(id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (a) => { this.activity.set(a); this.loading.set(false); },
        error: (err) => { this.error.set(err.message ?? 'Failed to load activity.'); this.loading.set(false); }
      });
  }

  back(): void { this.router.navigate(['/wellness/activities']); }

  markWatched(): void {
    const a = this.activity(); if (!a) return;
    this.activityService.markWatched(a.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toast.success('Marked as completed! 🎉');
          this.fetch(a.id);       // refresh hasAttended flag
          // Show feedback popup — only once per program (backend enforces uniqueness;
          // if already submitted, the popup will silently close)
          this.showFeedbackPopup.set(true);
        },
        error: (err) => this.toast.error(err.message ?? 'Failed to mark complete.')
      });
  }

  onFeedbackDismissed(): void {
    this.showFeedbackPopup.set(false);
  }

  leaveActivity(): void {
    const a = this.activity(); if (!a) return;
    this.activityService.leave(a.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toast.success('Left activity.');
          this.router.navigate(['/wellness/activities']);
        },
        error: (err) => this.toast.error(err.message ?? 'Failed to leave.')
      });
  }

  modeBadge(): { cls: string; icon: string; label: string } {
    const a = this.activity();
    if (a?.mode === 'Online') return { cls: 'bg-info-subtle text-info', icon: 'bi-play-btn', label: 'Online' };
    return { cls: 'bg-success-subtle text-success', icon: 'bi-geo-alt', label: 'Offline' };
  }

  statusLabel(): string {
    const map: Record<string, string> = { active:'Scheduled', pending:'In Progress', completed:'Completed', cancelled:'Cancelled', inactive:'Inactive' };
    return map[this.status()] ?? this.status();
  }
  statusClass(): string {
    switch (this.status()) {
      case 'active':    return 'bg-success-subtle text-success';
      case 'pending':   return 'bg-warning-subtle text-warning';
      case 'completed': return 'bg-primary-subtle text-primary';
      case 'cancelled': return 'bg-danger-subtle text-danger';
      default:          return 'bg-secondary-subtle text-secondary';
    }
  }
}
