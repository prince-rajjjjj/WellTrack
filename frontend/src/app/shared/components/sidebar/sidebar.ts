import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../core/services/auth';

interface NavLink {
  label: string;
  icon:  string;
  route: string;
}

interface NavSection {
  title: string;
  links: NavLink[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive, CommonModule],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.scss'
})
export class Sidebar {
  private auth = inject(AuthService);

  get userRole() { return this.auth.getCurrentRole() || ''; }

  // ── Sidebar sections per role ───────────────────────────────
 sections(): NavSection[] {
    switch (this.userRole) {

      case 'Admin':
        return [
          {
            title: 'Overview',
            links: [
              { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard' }
            ]
          },
          {
            title: 'User Management',
            links: [
              { label: 'All Users',  icon: 'bi-people',       route: '/identity/users' },
              { label: 'Audit Logs', icon: 'bi-shield-check', route: '/identity/audit-logs' }
            ]
          },
          {
            title: 'Wellness Programs',
            links: [
              { label: 'Programs',   icon: 'bi-calendar-event', route: '/wellness/programs' },
              { label: 'Activities', icon: 'bi-list-task',      route: '/wellness/activities' }
            ]
          },
          {
            title: 'Engagement',
            links: [
              { label: 'Engagement Tracker', icon: 'bi-heart-pulse', route: '/engagement/tracker' },
              { label: 'Feedback',           icon: 'bi-chat-dots',   route: '/engagement/feedback' }
            ]
          },
          {
            title: 'Compliance',
            links: [
              { label: 'Policies',           icon: 'bi-file-earmark-text', route: '/compliance/policies' },
              { label: 'Audits',             icon: 'bi-shield-check',      route: '/compliance/audits' },
              { label: 'Compliance Records', icon: 'bi-clipboard-check',   route: '/compliance/records' }
            ]
          },
          {
            title: 'Insights',
            links: [
              { label: 'Report',      icon: 'bi-graph-up',  route: '/reporting' }
            ]
          }
        ];

      case 'HR':
        return [
          {
            title: 'Overview',
            links: [
              { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard' }
            ]
          },
          {
            title: 'Wellness Programs',
            links: [
              { label: 'Programs',   icon: 'bi-calendar-event', route: '/wellness/programs' },
              { label: 'Activities', icon: 'bi-list-task',      route: '/wellness/activities' }
            ]
          },
          {
            title: 'Engagement',
            links: [
              { label: 'Engagement Tracker', icon: 'bi-heart-pulse', route: '/engagement/tracker' },
              { label: 'Feedback',           icon: 'bi-chat-dots',   route: '/engagement/feedback' }
            ]
          },
          {
            title: 'Compliance',
            links: [
              { label: 'Policies',           icon: 'bi-file-earmark-text', route: '/compliance/policies' },
              { label: 'Compliance Records', icon: 'bi-clipboard-check',   route: '/compliance/records' }
            ]
          },
          {
            title: 'Insights',
            links: [
              { label: 'Overview Report',      icon: 'bi-graph-up',  route: '/reporting' },
            ]
          }
        ];

      case 'Coach':
        return [
          {
            title: 'Overview',
            links: [
              { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard' }
            ]
          },
          {
            title: 'My Work',
            links: [
              { label: 'Activities', icon: 'bi-list-task',     route: '/wellness/activities' },
              { label: 'Programs',   icon: 'bi-calendar-event', route: '/wellness/programs' }
            ]
          },
          {
            title: 'Engagement',
            links: [
              { label: 'Feedback',      icon: 'bi-chat-dots', route: '/engagement/feedback' }
            ]
          }
        ];

      case 'ComplianceOfficer':
        return [
          {
            title: 'Overview',
            links: [
              { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard' }
            ]
          },
          {
            title: 'Compliance',
            links: [
              { label: 'Policies',           icon: 'bi-file-earmark-text', route: '/compliance/policies' },
              { label: 'Audits',             icon: 'bi-shield-check',      route: '/compliance/audits' },
              { label: 'Compliance Records', icon: 'bi-clipboard-check',   route: '/compliance/records' }
            ]
          }
        ];

      case 'Employee':
      default:
        return [
          {
            title: 'Overview',
            links: [
              { label: 'Dashboard', icon: 'bi-speedometer2', route: '/dashboard' }
            ]
          },
          {
            title: 'Wellness',
            links: [
              { label: 'Programs',   icon: 'bi-calendar-event', route: '/wellness/programs' },
              { label: 'Activities', icon: 'bi-list-task',      route: '/wellness/activities' }
            ]
          },
          {
            title: 'My Progress',
            links: [
              { label: 'My Goals',         icon: 'bi-bullseye', route: '/participation/goals' },
              { label: 'My Participation', icon: 'bi-activity', route: '/participation' }
            ]
          },
          {
            title: 'Engagement',
            links: [
              { label: 'Feedback',      icon: 'bi-chat-dots', route: '/engagement/feedback' }
            ]
          }
        ];
    }
  }
}