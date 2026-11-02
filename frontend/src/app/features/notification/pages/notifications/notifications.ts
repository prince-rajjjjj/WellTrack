import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NotificationService } from '../../services/notification';
import { Notification } from '../../models/notification.model';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './notifications.html',
  styleUrl: './notifications.scss'
})
export class NotificationsComponent implements OnInit {
  protected service = inject(NotificationService);

  loading = signal(false);
  activeFilter = signal<string>('all');
  filters = ['all', 'unread', 'read'];

  filtered = computed(() => {
    const groups = this.service.grouped();
    const filter = this.activeFilter();
    if (filter === 'all') return groups;

    return groups.map(g => ({
      label: g.label,
      items: g.items.filter(n => {
        if (filter === 'unread') return n.status === 'Active';
        if (filter === 'read')   return n.status !== 'Active';
        return n.category.toLowerCase() === filter.toLowerCase();
      })
    })).filter(g => g.items.length > 0);
  });

  ngOnInit(): void {
    this.refresh();
  }

  refresh(): void {
    this.loading.set(true);
    this.service.loadAll().subscribe({
      next:  () => this.loading.set(false),
      error: () => this.loading.set(false)
    });
  }

  markRead(n: Notification): void {
    if (n.status === 'Active') this.service.markAsRead(n.id).subscribe();
  }

  markAllRead(): void {
    this.service.markAllAsRead().subscribe();
  }

  delete(id: string, event: Event): void {
    event.stopPropagation();
    this.service.delete(id).subscribe();
  }
}