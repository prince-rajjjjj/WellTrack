import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ComplianceService } from '../../services/compliance';
import { AuthService } from '../../../../core/services/auth';
import { CreateAuditDto } from '../../models/compliance.model';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';

@Component({
  selector:    'app-audit-logs',
  standalone:  true,
  imports:     [CommonModule, FormsModule, Skeleton],
  // FormsModule already imported — used for ngModel on the reject reason textarea
  templateUrl: './audit-logs.html',
  styleUrl:    './audit-logs.scss',
})
export class AuditLogs implements OnInit {
  readonly svc  = inject(ComplianceService);
  private auth  = inject(AuthService);

  // Role-based capabilities:
  //   ComplianceOfficer  → can log audits (Create) and submit drafts
  //   Admin              → can approve / reject submitted audits (reject auto-closes)
  get isComplianceOfficer(): boolean { return this.auth.getCurrentRole() === 'ComplianceOfficer'; }
  get isAdmin():             boolean { return this.auth.getCurrentRole() === 'Admin'; }

  showForm = signal(false);
  scope    = '';
  findings = '';

  // ── Pagination (server-side) ──────────────────────────────────────
  // Source of truth: ComplianceService.auditsPage / auditsTotalCount.
  page     = signal(1);
  pageSize = 9;

  pagedAudits = computed(() => this.svc.auditsPage());
  totalCount  = computed(() => this.svc.auditsTotalCount());
  totalPages  = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.svc.loadPagedAudits(p, this.pageSize);
  }
  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  // Reject modal state
  rejectingId = signal<string | null>(null);
  rejectReason = '';

  ngOnInit(): void {
    // Full list backs the draftCount badge in the header; paged endpoint
    // drives the audit-card grid.
    if (this.svc.audits().length === 0) this.svc.loadAudits();
    this.svc.loadPagedAudits(1, this.pageSize);
  }

  openForm():  void { this.scope = ''; this.findings = ''; this.showForm.set(true); }
  closeForm(): void { this.showForm.set(false); }

  submit(): void {
    if (!this.scope.trim()) return;
    const dto: CreateAuditDto = {
      complianceOfficerId: this.auth.getCurrentUserId(),
      scope: this.scope,
      findings: this.findings || undefined
    };
    this.svc.createAudit(dto, () => this.closeForm());
  }

  // ── Workflow actions ────────────────────────────────────────────────
  submitDraft(id: string): void {
    this.svc.submitAudit(id);
  }

  approve(id: string): void {
    this.svc.approveAudit(id);
  }

  openReject(id: string): void {
    this.rejectReason = '';
    this.rejectingId.set(id);
  }
  cancelReject(): void {
    this.rejectingId.set(null);
  }
  confirmReject(): void {
    const id = this.rejectingId();
    if (!id || !this.rejectReason.trim()) return;
    this.svc.rejectAudit(id, this.rejectReason.trim());
    this.rejectingId.set(null);
  }

  statusColor(s: string): string {
    return { Draft: 'secondary', Submitted: 'warning', Approved: 'success', Rejected: 'danger', Closed: 'dark' }[s] ?? 'secondary';
  }
}
