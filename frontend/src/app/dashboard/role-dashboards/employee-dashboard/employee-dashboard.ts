import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { RouterLink }              from '@angular/router';
import { AuthService }              from '../../../core/services/auth';
import { GoalService }              from '../../../features/participation/services/goal';
import { ParticipationService }     from '../../../features/participation/services/participation';

/** Employee dashboard — wellness-focused, the original layout. */
@Component({
  selector:    'app-employee-dashboard',
  standalone:  true,
  imports:     [CommonModule, RouterLink, DatePipe],
  templateUrl: './employee-dashboard.html',
  styleUrl:    './employee-dashboard.scss',
})
export class EmployeeDashboard implements OnInit {
  readonly auth                 = inject(AuthService);
  readonly goalService          = inject(GoalService);
  readonly participationService = inject(ParticipationService);

  get userName(): string { return this.auth.getCurrentName(); }
  get userRole(): string { return this.auth.getCurrentRole(); }

  readonly today = new Date();

  readonly quickActions = [
    { label: 'My Goals',          icon: 'bi-bullseye',       route: '/participation/goals', color: 'text-primary'   },
    { label: 'Wellness Programs', icon: 'bi-activity',       route: '/wellness',            color: 'text-success'   },
    { label: 'My Participation',  icon: 'bi-person-check',   route: '/participation',       color: 'text-info'      },
    { label: 'Tracker',           icon: 'bi-bar-chart-line', route: '/engagement/tracker',  color: 'text-warning'   },
    { label: 'Notifications',     icon: 'bi-bell',           route: '/notifications',       color: 'text-secondary' },
    { label: 'My Profile',        icon: 'bi-person-circle',  route: '/identity/profile',    color: 'text-dark'      },
  ];

  ngOnInit(): void {
    if (this.goalService.goals().length === 0) {
      this.goalService.loadAll();
    }
    if (this.participationService.participations().length === 0) {
      this.participationService.loadMine();
    }
  }
}
