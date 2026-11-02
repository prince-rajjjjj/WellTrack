import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReportingService } from '../../services/reporting';
import { EngagementScoreReport } from '../../models/report.model';

@Component({
  selector: 'app-goal-report',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './goal-report.html',
  styleUrl: './goal-report.scss',
})
export class GoalReport implements OnInit {
  private svc = inject(ReportingService);

  loading = signal(true);
  error   = signal<string | null>(null);
  report  = signal<EngagementScoreReport | null>(null);

  ngOnInit(): void {
    this.svc.getEngagementScores().subscribe({
      next:  data => { this.report.set(data); this.loading.set(false); },
      error: err  => { this.error.set(err.message ?? 'Failed to load.'); this.loading.set(false); }
    });
  }

  total(): number {
    const r = this.report();
    return r ? (r.highlyEngaged + r.moderatelyEngaged + r.lowEngaged) || 1 : 1;
  }

  highPct():  number { return ((this.report()?.highlyEngaged    ?? 0) / this.total()) * 100; }
  medPct():   number { return ((this.report()?.moderatelyEngaged ?? 0) / this.total()) * 100; }
  lowPct():   number { return ((this.report()?.lowEngaged        ?? 0) / this.total()) * 100; }

  scoreBadge(score: number): string {
    if (score >= 7) return 'bg-success-subtle text-success';
    if (score >= 4) return 'bg-warning-subtle text-warning';
    return 'bg-danger-subtle text-danger';
  }
}