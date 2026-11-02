import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuditLogService } from '../../services/audit-log';
import { UserAdminService } from '../../services/user-admin';

@Component({
  selector: 'app-audit-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './audit-logs.html',
  styleUrl: './audit-logs.scss'
})
export class AuditLogsComponent implements OnInit {
  protected logService  = inject(AuditLogService);
  protected userService = inject(UserAdminService);

  loading    = signal(false);
  error      = signal<string | null>(null);
  searchTerm = signal('');

  // Build a userId → name lookup so we can show names instead of GUIDs
  userMap = computed(() => {
    const map: Record<string, { name: string; email: string }> = {};
    for (const u of this.userService.users()) {
      map[u.id] = { name: u.name, email: u.email };
    }
    return map;
  });

  ngOnInit(): void {
    // Make sure user list is loaded so we can map userIds to names
    if (this.userService.users().length === 0) {
      this.userService.loadUsers().subscribe();
    }
    this.fetchLogs();
  }

  fetchLogs(): void {
    this.loading.set(true);
    this.error.set(null);

    this.logService.loadLogs(this.searchTerm() || undefined).subscribe({
      next:  () => this.loading.set(false),
      error: (err) => {
        this.error.set(err.message ?? 'Failed to load audit logs.');
        this.loading.set(false);
      }
    });
  }

  onSearchChange(value: string): void {
    this.searchTerm.set(value);
    this.logService.setPage(1);
    this.fetchLogs();
  }

  clearSearch(): void {
    this.searchTerm.set('');
    this.logService.setPage(1);
    this.fetchLogs();
  }

  goToPage(p: number): void {
    this.logService.setPage(p);
    this.fetchLogs();
  }

  pageNumbers(): number[] {
    const total = this.logService.totalPages();
    const current = this.logService.page();
    const pages: number[] = [];
    for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) {
      pages.push(i);
    }
    return pages;
  }

  // Get user name from cached userMap
  userName(userId: string): string {
    return this.userMap()[userId]?.name ?? 'Unknown User';
  }

  userEmail(userId: string): string {
    return this.userMap()[userId]?.email ?? userId;
  }

  // Color-code actions
  actionBadgeClass(action: string): string {
    const lower = action.toLowerCase();
    if (lower.includes('login'))    return 'bg-success-subtle text-success';
    if (lower.includes('logout'))   return 'bg-secondary-subtle text-secondary';
    if (lower.includes('create'))   return 'bg-primary-subtle text-primary';
    if (lower.includes('update') || lower.includes('change')) return 'bg-warning-subtle text-warning';
    if (lower.includes('delete'))   return 'bg-danger-subtle text-danger';
    if (lower.includes('reset'))    return 'bg-info-subtle text-info';
    return 'bg-light text-muted';
  }

  actionIcon(action: string): string {
    const lower = action.toLowerCase();
    if (lower.includes('login'))    return 'bi-box-arrow-in-right';
    if (lower.includes('logout'))   return 'bi-box-arrow-right';
    if (lower.includes('create'))   return 'bi-plus-circle';
    if (lower.includes('update') || lower.includes('change')) return 'bi-pencil';
    if (lower.includes('delete'))   return 'bi-trash';
    if (lower.includes('reset'))    return 'bi-key';
    return 'bi-activity';
  }
}