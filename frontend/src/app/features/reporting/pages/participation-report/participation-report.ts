import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReportingService } from '../../services/reporting';
import { ParticipationTrendReport, MonthlyTrend } from '../../models/report.model';

@Component({
  selector: 'app-participation-report',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './participation-report.html',
  styleUrl: './participation-report.scss',
})
export class ParticipationReport implements OnInit {
  private svc = inject(ReportingService);

  loading = signal(true);
  error   = signal<string | null>(null);
  report  = signal<ParticipationTrendReport | null>(null);

  // Filter out the current month — only show completed past months
  pastMonthTrends = computed<MonthlyTrend[]>(() => {
    const trends = this.report()?.monthlyTrends ?? [];
    const now = new Date();
    const months = ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'];
    const currentMonthLabel = `${months[now.getMonth()]} ${now.getFullYear()}`;
    return trends.filter(t => t.month !== currentMonthLabel);
  });

  ngOnInit(): void {
    this.svc.getParticipationTrends().subscribe({
      next:  data => { this.report.set(data); this.loading.set(false); },
      error: err  => { this.error.set(err.message ?? 'Failed to load.'); this.loading.set(false); }
    });
  }
}