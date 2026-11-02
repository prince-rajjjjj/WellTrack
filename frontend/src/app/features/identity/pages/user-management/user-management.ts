import { Component, inject, signal, computed, OnInit, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs/operators';
import { UserAdminService } from '../../services/user-admin';
import { UserProfile } from '../../models/user.model';
import { CreateUserModalComponent } from '../../components/create-user-modal/create-user-modal';
import { EditUserModalComponent } from '../../components/edit-user-modal/edit-user-modal';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule, CreateUserModalComponent, EditUserModalComponent],
  templateUrl: './user-management.html',
  styleUrl: './user-management.scss'
})
export class UserManagementComponent implements OnInit {
  protected userService = inject(UserAdminService);
  private destroyRef    = inject(DestroyRef);

  loading    = signal(false);
  error      = signal<string | null>(null);
  successMsg = signal<string | null>(null);

  searchTerm = signal('');
  roleFilter = signal('');
  page       = signal(1);
  pageSize   = 10;

  showCreateModal = signal(false);
  showEditModal   = signal(false);
  selectedUser    = signal<UserProfile | null>(null);

  filteredUsers = computed(() => {
    let result = this.userService.users();
    const role = this.roleFilter();
    if (role) result = result.filter(u => u.role === role);

    const term = this.searchTerm().toLowerCase().trim();
    if (term) {
      result = result.filter(u =>
        u.name.toLowerCase().includes(term) ||
        u.email.toLowerCase().includes(term)
      );
    }
    return result;
  });

  totalCount = computed(() => this.filteredUsers().length);
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  pagedUsers = computed(() => {
    const start = (this.page() - 1) * this.pageSize;
    return this.filteredUsers().slice(start, start + this.pageSize);
  });

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.userService
      .loadUsers()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next:  () => this.loading.set(false),
        error: (err) => {
          this.error.set(err.message ?? 'Failed to load users.');
          this.loading.set(false);
        }
      });
  }

  onSearchChange(value: string): void { this.searchTerm.set(value); this.page.set(1); }
  onRoleChange(value: string): void   { this.roleFilter.set(value); this.page.set(1); }
  clearSearch(): void { this.searchTerm.set(''); this.page.set(1); }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
  }

  pageNumbers(): number[] {
    const total = this.totalPages();
    const current = this.page();
    const pages: number[] = [];
    for (let i = Math.max(1, current - 2); i <= Math.min(total, current + 2); i++) {
      pages.push(i);
    }
    return pages;
  }

  // ── Modals (no fetchUsers needed — modal already reloaded) ──
  openCreate(): void { this.showCreateModal.set(true); }

  onCreateClosed(created: boolean): void {
    this.showCreateModal.set(false);
    if (created) this.flashSuccess('User created.');
  }

  openEdit(user: UserProfile): void {
    this.selectedUser.set(user);
    this.showEditModal.set(true);
  }

  onEditClosed(updated: boolean): void {
    this.showEditModal.set(false);
    this.selectedUser.set(null);
    if (updated) this.flashSuccess('User updated.');
  }

  // ── Inline actions — chain with reload ──
  toggleStatus(user: UserProfile): void {
    const action = user.status === 'Active' ? 'deactivate' : 'activate';
    if (!confirm(`Are you sure you want to ${action} ${user.name}?`)) return;

    this.userService.toggleStatus(user)
      .pipe(
        switchMap(() => this.userService.loadUsers()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next:  () => this.flashSuccess(`User ${action}d.`),
        error: (err) => this.error.set(err.message ?? 'Failed.')
      });
  }

  deleteUser(user: UserProfile): void {
    if (!confirm(`Permanently delete ${user.name}? This cannot be undone.`)) return;

    this.userService.deleteUser(user.id)
      .pipe(
        switchMap(() => this.userService.loadUsers()),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe({
        next:  () => this.flashSuccess('User deleted.'),
        error: (err) => this.error.set(err.message ?? 'Failed.')
      });
  }

  private flashSuccess(msg: string): void {
    this.successMsg.set(msg);
    setTimeout(() => this.successMsg.set(null), 3000);
  }

  clearMessages(): void {
    this.error.set(null);
    this.successMsg.set(null);
  }

  roleBadgeClass(role: string): string {
    switch (role) {
      case 'Admin':             return 'bg-danger-subtle text-danger';
      case 'HR':                return 'bg-primary-subtle text-primary';
      case 'Coach':             return 'bg-info-subtle text-info';
      case 'ComplianceOfficer': return 'bg-warning-subtle text-warning';
      default:                  return 'bg-success-subtle text-success';
    }
  }

  statusBadgeClass(status: string): string {
    switch (status) {
      case 'Active':   return 'bg-success-subtle text-success';
      case 'Inactive': return 'bg-secondary-subtle text-secondary';
      case 'Pending':  return 'bg-warning-subtle text-warning';
      default:         return 'bg-light text-muted';
    }
  }
}