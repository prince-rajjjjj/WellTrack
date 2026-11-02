import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './core/services/auth';
import { NotificationService } from './features/notification/services/notification';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: `<router-outlet />`,
  styles: [`:host { display: block; height: 100%; }`]
})
export class App implements OnInit {
  private auth = inject(AuthService);
  private notificationService = inject(NotificationService);

  ngOnInit(): void {
    if (this.auth.isLoggedIn()) {
      this.notificationService.startPolling();
      this.notificationService.loadAll().subscribe();
    }
  }
}