import {
  Component,
  Input,
  AfterViewInit,
  OnChanges,
  OnDestroy,
  ViewChild,
  ElementRef,
  SimpleChanges,
} from '@angular/core';
import { Chart, ChartConfiguration, registerables } from 'chart.js';
import { ScoreBreakdownDto, EngagementSummaryDto } from '../../models/engagement.model';

Chart.register(...registerables);

/**
 * Reusable chart component with two modes:
 *
 *  'breakdown'    → horizontal bar chart showing all 5 score factors (employee cards)
 *  'distribution' → doughnut chart showing High / Medium / Low split (HR summary)
 */
@Component({
  selector: 'app-engagement-chart',
  standalone: true,
  imports: [],
  templateUrl: './engagement-chart.html',
  styleUrl: './engagement-chart.scss',
})
export class EngagementChart implements AfterViewInit, OnChanges, OnDestroy {
  @ViewChild('chartCanvas') chartCanvas!: ElementRef<HTMLCanvasElement>;

  @Input() type: 'breakdown' | 'distribution' = 'breakdown';
  @Input() breakdownData?: ScoreBreakdownDto;
  @Input() summaryData?: EngagementSummaryDto;

  private chart?: Chart;
  private ready = false;

  ngAfterViewInit(): void {
    this.ready = true;
    this.render();
  }

  ngOnChanges(_: SimpleChanges): void {
    if (this.ready) this.render();
  }

  ngOnDestroy(): void {
    this.chart?.destroy();
  }

  private render(): void {
    this.chart?.destroy();
    const ctx = this.chartCanvas?.nativeElement?.getContext('2d');
    if (!ctx) return;

    if (this.type === 'breakdown' && this.breakdownData) {
      this.chart = new Chart(ctx, this.breakdownConfig(this.breakdownData));
    } else if (this.type === 'distribution' && this.summaryData) {
      this.chart = new Chart(ctx, this.distributionConfig(this.summaryData));
    }
  }

  // ── Horizontal bar — 5 factors normalised to % of their max ──────────
  private breakdownConfig(d: ScoreBreakdownDto): ChartConfiguration {
    const maxes = [30, 25, 25, 10, 10];
    const raw   = [d.attendanceScore, d.activityScore, d.goalScore, d.feedbackScore, d.consistencyScore];
    const pct   = raw.map((v, i) => Math.round((v / maxes[i]) * 100));

    return {
      type: 'bar',
      data: {
        labels: ['Attendance /30', 'Activities /25', 'Goals /25', 'Feedback /10', 'Consistency /10'],
        datasets: [{
          label: 'Score %',
          data: pct,
          backgroundColor: [
            'rgba(76,175,80,0.8)',
            'rgba(33,150,243,0.8)',
            'rgba(156,39,176,0.8)',
            'rgba(255,152,0,0.8)',
            'rgba(0,188,212,0.8)',
          ],
          borderRadius: 5,
          barThickness: 16,
        }],
      },
      options: {
        indexAxis: 'y',
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: false },
          tooltip: {
            callbacks: {
              label: (ctx) => ` ${ctx.parsed.x}%  (${raw[ctx.dataIndex]} pts)`,
            },
          },
        },
        scales: {
          x: { min: 0, max: 100, ticks: { callback: (v) => `${v}%` } },
        },
      },
    };
  }

  // ── Doughnut — High / Medium / Low distribution ───────────────────────
  private distributionConfig(d: EngagementSummaryDto): ChartConfiguration {
    return {
      type: 'doughnut',
      data: {
        labels: ['High ≥75', 'Medium 40–74', 'Low <40'],
        datasets: [{
          data: [d.highEngagement, d.mediumEngagement, d.lowEngagement],
          backgroundColor: [
            'rgba(76,175,80,0.85)',
            'rgba(255,193,7,0.85)',
            'rgba(244,67,54,0.85)',
          ],
          borderWidth: 2,
          borderColor: '#fff',
        }],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { position: 'bottom' },
          tooltip: {
            callbacks: {
              label: (ctx) => ` ${ctx.label}: ${ctx.parsed} employees`,
            },
          },
        },
      },
    };
  }
}
