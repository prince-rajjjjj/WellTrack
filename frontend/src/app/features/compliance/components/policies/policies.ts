import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplianceService } from '../../services/compliance';
import { AuthService } from '../../../../core/services/auth';
import { ConfirmDialog } from '../../../../shared/components/confirm-dialog/confirm-dialog';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';
import { CreatePolicyDto } from '../../models/compliance.model';

@Component({
  selector:    'app-policies',
  standalone:  true,
  imports:     [CommonModule, FormsModule, ConfirmDialog, Skeleton],
  templateUrl: './policies.html',
  styleUrl:    './policies.scss',
})
export class Policies implements OnInit {
  readonly svc = inject(ComplianceService);
  private auth = inject(AuthService);

  // Only Admin can create/edit/toggle policies; ComplianceOfficer + HR are read-only.
  get canEdit(): boolean { return this.auth.getCurrentRole() === 'Admin'; }

  showForm  = signal(false);
  severities = ['Low', 'Medium', 'High', 'Critical'];
  categories = ['Safety', 'Data Privacy', 'Attendance', 'Conduct', 'Health', 'Other'];

  // ── Pagination (server-side) ──────────────────────────────────────
  // Source of truth: ComplianceService.policiesPage / policiesTotalCount.
  page     = signal(1);
  pageSize = 10;

  pagedPolicies = computed(() => this.svc.policiesPage());
  totalCount    = computed(() => this.svc.policiesTotalCount());
  totalPages    = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.svc.loadPagedPolicies(p, this.pageSize);
  }
  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  form: CreatePolicyDto = this._blank();

  ngOnInit(): void {
    // Full list keeps activeCount and the policy dropdown in compliance-records
    // populated; paged endpoint drives the table display.
    if (this.svc.policies().length === 0) this.svc.loadPolicies();
    this.svc.loadPagedPolicies(1, this.pageSize);
  }

  openForm():  void { this.form = this._blank(); this.showForm.set(true); }
  closeForm(): void { this.showForm.set(false); }

  submit(): void {
    if (!this.form.policyName.trim()) return;
    this.svc.createPolicy(this.form, () => this.closeForm());
  }

  // Confirm-then-delete state for the inline confirmation modal
  deletingId = signal<string | null>(null);
  deletingName = '';

  askDelete(id: string, name: string): void {
    this.deletingId.set(id);
    this.deletingName = name;
  }
  cancelDelete(): void { this.deletingId.set(null); }
  confirmDelete(): void {
    const id = this.deletingId();
    if (!id) return;
    // Close modal on either success OR error so the user is never stuck.
    // The service shows a precise error toast (with HTTP status) on failure.
    this.svc.deletePolicy(id, () => this.cancelDelete(), () => this.cancelDelete());
  }

  severityColor(s: string): string {
    return { Low: 'success', Medium: 'warning', High: 'danger', Critical: 'dark' }[s] ?? 'secondary';
  }

  private _blank(): CreatePolicyDto {
    return { policyName: '', description: '', category: 'Safety', severity: 'Medium' };
  }
}
