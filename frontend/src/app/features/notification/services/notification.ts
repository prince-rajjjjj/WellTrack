import { Injectable, inject, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, Subscription, fromEvent, interval, merge } from 'rxjs';
import { filter, map, startWith, switchMap, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { ToastService } from '../../../shared/services/toast.service';
import { Notification } from '../models/notification.model';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private http  = inject(HttpClient);
  private toast = inject(ToastService);
  private base  = environment.apiUrl;

  // ─── State ───────────────────────────────────────────────
  notifications = signal<Notification[]>([]);
  unreadCount   = signal(0);
  private lastSeenCount = 0;

  // ─── Derived ─────────────────────────────────────────────
  /** Unread notifications — newest first. */
  unread = computed(() =>
    [...this.notifications()]
      .filter(n => n.status === 'Active')
      .sort((a, b) => this.parseUtc(b.createdAt).getTime() - this.parseUtc(a.createdAt).getTime())
  );

  /** Notifications grouped by Today / Yesterday / This Week / Older — newest first within each group. */
  grouped = computed(() => {
    const groups: { label: string; items: Notification[] }[] = [
      { label: 'Today',     items: [] },
      { label: 'Yesterday', items: [] },
      { label: 'This Week', items: [] },
      { label: 'Older',     items: [] }
    ];

    const now = new Date();
    const today     = new Date(now.getFullYear(), now.getMonth(), now.getDate());
    const yesterday = new Date(today.getTime() - 86_400_000);
    const weekAgo   = new Date(today.getTime() - 7 * 86_400_000);

    // ✅ Newest first — sort by createdAt DESC before bucketing
    const sorted = [...this.notifications()].sort(
      (a, b) => this.parseUtc(b.createdAt).getTime() - this.parseUtc(a.createdAt).getTime()
    );

    for (const n of sorted) {
      const created = this.parseUtc(n.createdAt);
      if      (created >= today)     groups[0].items.push(n);
      else if (created >= yesterday) groups[1].items.push(n);
      else if (created >= weekAgo)   groups[2].items.push(n);
      else                           groups[3].items.push(n);
    }

    return groups.filter(g => g.items.length > 0);
  });

  // ─── Smart Polling ───────────────────────────────────────
  private pollSub: Subscription | null = null;

  startPolling(): void {
    if (this.pollSub) return;

    this.pollSub = merge(
      interval(10_000),
      fromEvent(document, 'visibilitychange')
    ).pipe(
      startWith(0),
      filter(() => !document.hidden),
      switchMap(() => this.fetchUnreadCount())
    ).subscribe();
  }

  stopPolling(): void {
    this.pollSub?.unsubscribe();
    this.pollSub = null;
    this.lastSeenCount = 0;
    this.notifications.set([]);
    this.unreadCount.set(0);
  }

  private fetchUnreadCount(): Observable<number> {
    return this.http
      .get<ApiResponse<{ count: number }>>(`${this.base}/api/v1/notifications/unread-count`)
      .pipe(
        map(res => res.data?.count ?? 0),
        tap(count => {
          // Show toast if count increased
          if (count > this.lastSeenCount && this.lastSeenCount > 0) {
            const diff = count - this.lastSeenCount;
            this.toast.info(
              `${diff} new notification${diff > 1 ? 's' : ''}`,
              '🔔 New'
            );
          }

          this.lastSeenCount = count;
          this.unreadCount.set(count);

          // ✅ If the local notifications list is out of sync, refresh it
          const localUnread = this.notifications().filter(n => n.status === 'Active').length;
          if (count !== localUnread) {
            this.loadAll().subscribe();
          }
        })
      );
  }

  // ─── REST methods ────────────────────────────────────────
  loadAll(): Observable<Notification[]> {
    return this.http
      .get<ApiResponse<Notification[]>>(`${this.base}/api/v1/notifications`)
      .pipe(
        map(res => res.data ?? []),
        tap(list => {
          this.notifications.set(list);
          const unread = list.filter(n => n.status === 'Active').length;
          this.unreadCount.set(unread);
          this.lastSeenCount = unread;
        })
      );
  }

  markAsRead(id: string): Observable<void> {
    return this.http
      .patch<ApiResponse<object>>(`${this.base}/api/v1/notifications/${id}/read`, {})
      .pipe(
        map(() => undefined),
        tap(() => {
          this.notifications.update(list =>
            list.map(n => n.id === id ? { ...n, status: 'Inactive' as const } : n)
          );
          this.unreadCount.update(n => Math.max(0, n - 1));
          this.lastSeenCount = Math.max(0, this.lastSeenCount - 1);
        })
      );
  }

  markAllAsRead(): Observable<void> {
    return this.http
      .patch<ApiResponse<object>>(`${this.base}/api/v1/notifications/mark-all-read`, {})
      .pipe(
        map(() => undefined),
        tap(() => {
          this.notifications.update(list =>
            list.map(n => ({ ...n, status: 'Inactive' as const }))
          );
          this.unreadCount.set(0);
          this.lastSeenCount = 0;
        })
      );
  }

  delete(id: string): Observable<void> {
    return this.http
      .delete<ApiResponse<object>>(`${this.base}/api/v1/notifications/${id}`)
      .pipe(
        map(() => undefined),
        tap(() => {
          this.notifications.update(list => list.filter(n => n.id !== id));
        })
      );
  }

  // ─── UI helpers ──────────────────────────────────────────

  /** Force UTC parsing — backend sends UTC but often without 'Z'. */
  private parseUtc(date: string): Date {
    const iso = /[Zz]|[+-]\d{2}:?\d{2}$/.test(date) ? date : date + 'Z';
    return new Date(iso);
  }

  timeAgo(date: string): string {
    const ms  = Date.now() - this.parseUtc(date).getTime();
    const sec = Math.max(0, Math.floor(ms / 1000));
    if (sec < 60)     return 'just now';
    if (sec < 3600)   return `${Math.floor(sec / 60)} min ago`;
    if (sec < 86400)  return `${Math.floor(sec / 3600)} hr ago`;
    if (sec < 604800) return `${Math.floor(sec / 86400)} day ago`;
    return this.parseUtc(date).toLocaleString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }

  /** Full date+time for tooltip / detail view (e.g. "13 May 2026, 03:25 PM"). */
  fullDateTime(date: string): string {
    return this.parseUtc(date).toLocaleString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric',
      hour: '2-digit', minute: '2-digit', hour12: true
    });
  }

  categoryIcon(category: string): string {
    switch (category.toLowerCase()) {
      case 'participation': return 'bi-activity';
      case 'goal':          return 'bi-bullseye';
      case 'compliance':    return 'bi-shield-check';
      case 'activity':      return 'bi-calendar-event';
      case 'security':      return 'bi-shield-lock';
      case 'program':       return 'bi-collection';
      default:              return 'bi-bell';
    }
  }

  categoryColor(category: string): string {
    switch (category.toLowerCase()) {
      case 'participation': return 'text-primary';
      case 'goal':          return 'text-success';
      case 'compliance':    return 'text-warning';
      case 'activity':      return 'text-info';
      case 'security':      return 'text-danger';
      default:              return 'text-secondary';
    }
  }
}