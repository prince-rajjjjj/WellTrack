import { Component, inject, computed } from '@angular/core';
import { RouterLink }        from '@angular/router';
import { AuthService }       from '../../../core/services/auth';
import { NotificationBell }  from '../../../features/notification/components/notification-bell/notification-bell';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [RouterLink, NotificationBell],
  templateUrl: './navbar.html',
  styleUrl: './navbar.scss'
})
export class Navbar {
  readonly auth = inject(AuthService);

  // ── Reactive: pulls from the signal in AuthService ──
  userName = this.auth.name;                                   // signal
  initial  = computed(() => this.userName().charAt(0).toUpperCase());

  // Role is from the JWT — never changes during a session
  get userRole(): string { return this.auth.getCurrentRole(); }

  logout(): void {
    this.auth.logout();
  }
}