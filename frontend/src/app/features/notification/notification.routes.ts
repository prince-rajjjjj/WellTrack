import { Routes } from '@angular/router';

/**
 * Notification module routes — loaded inside MainLayout.
 * Member 7: Add your notification pages here.
 * Path prefix: /notifications/...
 *
 * API endpoints (reference):
 *   GET   /api/v1/notifications
 *   PATCH /api/v1/notifications/:id/read
 *   PATCH /api/v1/notifications/read-all
 */
export const notificationRoutes: Routes = [
  { path: '', loadComponent: () => import('./pages/notifications/notifications').then(m => m.NotificationsComponent) }
];
