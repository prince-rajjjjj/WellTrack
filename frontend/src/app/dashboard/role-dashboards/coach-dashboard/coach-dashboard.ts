import { Component, computed, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink }              from '@angular/router';
import { AuthService }              from '../../../core/services/auth';
import { ActivityService }          from '../../../features/wellness/services/activity';
import { ActivityDto }              from '../../../features/wellness/models/activity.model';

interface StatCard { label: string; value: number; sub: string; icon: string; tone: string; route?: string; }

/** Coach dashboard — focus on sessions assigned to them. */
@Component({
  selector:    'app-coach-dashboard',
  standalone:  true,
  imports:     [CommonModule, RouterLink, DatePipe],
  templateUrl: './coach-dashboard.html',
  styleUrl:    './coach-dashboard.scss',
})
export class CoachDashboard implements OnInit {
  private auth         = inject(AuthService);
  readonly activitySvc = inject(ActivityService);

  get userName():    string { return this.auth.getCurrentName(); }
  get currentUserId(): string { return this.auth.getCurrentUserId(); }
  readonly today = new Date();

  /** Activities where this coach is assigned. */
  readonly myActivities = computed(() =>
    this.activitySvc.activities().filter(a => a.assignedCoachID === this.currentUserId)
  );

  /** Today's schedule — activities happening on the current calendar day. */
  readonly todaysSchedule = computed<ActivityDto[]>(() => {
    const todayKey = new Date().toISOString().slice(0, 10); // YYYY-MM-DD
    return this.myActivities()
      .filter(a => (a.date ?? '').slice(0, 10) === todayKey)
      .sort((a, b) => (a.date ?? '').localeCompare(b.date ?? ''));
  });

  readonly stats = computed<StatCard[]>(() => {
    const all  = this.myActivities();
    const todays = this.todaysSchedule().length;
    const upcoming = all.filter(a => new Date(a.date ?? '').getTime() > Date.now()).length;
    const totalEnrolled = all.reduce((sum, a) => sum + (a.enrolledCount ?? 0), 0);
    const totalAttended = all.reduce((sum, a) => sum + (a.attendedCount ?? 0), 0);
    return [
      { label: "Today's Sessions", value: todays,        sub: 'Scheduled for today',
        icon: 'bi-calendar-day',  tone: 'primary', route: '/wellness/activities' },
      { label: 'Upcoming',         value: upcoming,      sub: 'Future sessions',
        icon: 'bi-calendar-week', tone: 'warning', route: '/wellness/activities' },
      { label: 'Participants',     value: totalEnrolled, sub: 'Across all sessions',
        icon: 'bi-people',        tone: 'success', route: '/wellness/activities' },
      { label: 'Attended',         value: totalAttended, sub: 'Marked present',
        icon: 'bi-check-circle',  tone: 'info',    route: '/wellness/activities' },
    ];
  });

  ngOnInit(): void {
    if (this.activitySvc.activities().length === 0) {
      this.activitySvc.loadActivities().subscribe();
    }
  }

  formatTime(iso: string): string {
    if (!iso) return '';
    const d = new Date(iso);
    return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }
}
