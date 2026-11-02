import { Component, DestroyRef, inject, signal, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { forkJoin, Subscription } from 'rxjs';
import { ReportingService } from '../../services/reporting';
import { ProgramReport, EngagementReport, ComplianceReport, UserDistribution } from '../../models/report.model';

@Component({
  selector: 'app-overview-report',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './overview-report.html',
  styleUrl: './overview-report.scss',
})
export class OverviewReport implements OnInit {
  private svc       = inject(ReportingService);
  private destroyRef = inject(DestroyRef);
  private loadSub?: Subscription;

  loading    = signal(true);
  error      = signal<string | null>(null);
  lastLoaded = signal<Date | null>(null);

  programs   = signal<ProgramReport | null>(null);
  engagement = signal<EngagementReport | null>(null);
  compliance = signal<ComplianceReport | null>(null);
  userDist   = signal<UserDistribution[]>([]);

  ngOnInit(): void { this.load(); }

  load(): void {
    // Cancel any in-flight request before starting a new one
    this.loadSub?.unsubscribe();

    this.loading.set(true);
    this.error.set(null);
    // Clear stale data so placeholders show skeletons not old numbers
    this.programs.set(null);
    this.engagement.set(null);
    this.compliance.set(null);
    this.userDist.set([]);

    this.loadSub = forkJoin({
      programs:   this.svc.getPrograms(),
      engagement: this.svc.getEngagement(),
      compliance: this.svc.getCompliance(),
      userDist:   this.svc.getUserDistribution(),
    })
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe({
      next: res => {
        this.programs.set(res.programs);
        this.engagement.set(res.engagement);
        this.compliance.set(res.compliance);
        this.userDist.set(res.userDist);
        this.loading.set(false);
        this.lastLoaded.set(new Date());
      },
      error: err => {
        this.error.set(err.message ?? 'Failed to load reports.');
        this.loading.set(false);
      }
    });
  }

  // ── Helpers ────────────────────────────────────────────────────────
  /** Compliance rate as a safe 0–100 number */
  get complianceRate(): number { return this.compliance()?.complianceRate ?? 0; }
  /** Avg rating as a 0–5 number */
  get avgRating(): number { return this.engagement()?.averageRating ?? 0; }
  /** Avg rating bar width as % of 5 */
  get avgRatingPct(): number { return (this.avgRating / 5) * 100; }

  roleClass(role: string): string {
    const map: Record<string, string> = {
      Admin: 'bg-danger', HR: 'bg-primary',
      Coach: 'bg-info', ComplianceOfficer: 'bg-warning', Employee: 'bg-success'
    };
    return map[role] ?? 'bg-secondary';
  }

  roleBadge(role: string): string {
    const map: Record<string, string> = {
      Admin: 'bg-danger-subtle text-danger',
      HR: 'bg-primary-subtle text-primary',
      Coach: 'bg-info-subtle text-info',
      ComplianceOfficer: 'bg-warning-subtle text-warning',
      Employee: 'bg-success-subtle text-success'
    };
    return map[role] ?? 'bg-secondary-subtle text-secondary';
  }
}
