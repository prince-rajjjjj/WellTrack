import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, switchMap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { ToastService } from '../../../shared/services/toast.service';
import {
  Audit, ComplianceRecord, CreateAuditDto,
  CreateComplianceRecordDto, CreatePolicyDto, Policy,
} from '../models/compliance.model';

@Injectable({ providedIn: 'root' })
export class ComplianceService {
  private http  = inject(HttpClient);
  private toast = inject(ToastService);
  private base  = `${environment.apiUrl}/api/v1`;

  // ── Policies ────────────────────────────────────────────────────────────────
  // _policies holds the FULL list (used for selectablePolicies dropdown,
  // activeCount badge, and the policy-name lookup in compliance-records).
  // _policiesPage holds only the current page for the policies table display.
  private _policies          = signal<Policy[]>([]);
  private _policiesPage      = signal<Policy[]>([]);
  private _policiesTotalCount = signal(0);
  private _policiesLoading   = signal(false);
  // In-flight flags dedupe duplicate concurrent calls — the dashboard +
  // policies page +  compliance-records all calling loadPolicies() in the
  // same tick used to fire 3 identical HTTP requests. Now they share one.
  private _policiesLoadInFlight     = false;
  private _policiesPagedInFlight    = false;
  readonly policies          = this._policies.asReadonly();
  readonly policiesPage      = this._policiesPage.asReadonly();
  readonly policiesTotalCount = this._policiesTotalCount.asReadonly();
  readonly policiesLoading   = this._policiesLoading.asReadonly();
  readonly activeCount       = computed(() => this._policies().filter(p => p.status === 'Active').length);

  loadPolicies(): void {
    if (this._policiesLoadInFlight) return;
    this._policiesLoadInFlight = true;
    this._policiesLoading.set(true);
    // Backend now returns PagedResult; request a large pageSize for the
    // "give me everything" use case (dropdowns, lookups, count badges).
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this.http.get<ApiResponse<PagedResult<Policy>>>(`${this.base}/policies`, { params })
      .pipe(finalize(() => {
        this._policiesLoading.set(false);
        this._policiesLoadInFlight = false;
      }))
      .subscribe({
        next: res => this._policies.set(res.data?.items ?? []),
        error: () => this.toast.error('Failed to load policies.')
      });
  }

  /** Server-side paginated policies — fills _policiesPage + _policiesTotalCount. */
  loadPagedPolicies(page: number, pageSize: number): void {
    if (this._policiesPagedInFlight) return;
    this._policiesPagedInFlight = true;
    this._policiesLoading.set(true);
    const params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    this.http.get<ApiResponse<PagedResult<Policy>>>(`${this.base}/policies`, { params })
      .pipe(finalize(() => {
        this._policiesLoading.set(false);
        this._policiesPagedInFlight = false;
      }))
      .subscribe({
        next: res => {
          if (res.data) {
            this._policiesPage.set(res.data.items);
            this._policiesTotalCount.set(res.data.totalCount);
          }
        },
        error: () => this.toast.error('Failed to load policies.')
      });
  }

  createPolicy(dto: CreatePolicyDto, onSuccess?: () => void): void {
    this._policiesLoading.set(true);
    this.http.post<ApiResponse<Policy>>(`${this.base}/policies`, dto)
      .pipe(finalize(() => this._policiesLoading.set(false)))
      .subscribe({
        next: res => {
          // Update both the full list (for activeCount/dropdowns) and the
          // current page view so the user sees their new policy immediately.
          this._policies.update(list => [...list, res.data!]);
          this._policiesPage.update(list => [res.data!, ...list]);
          this._policiesTotalCount.update(n => n + 1);
          this.toast.success('Policy created.');
          onSuccess?.();
        },
        error: () => this.toast.error('Failed to create policy.')
      });
  }

  deletePolicy(id: string, onSuccess?: () => void, onError?: () => void): void {
    this.http.delete<ApiResponse<object>>(`${this.base}/policies/${id}`)
      .subscribe({
        next: () => {
          // Remove from both the full list and the current page view.
          this._policies.update(list => list.filter(p => p.id !== id));
          this._policiesPage.update(list => list.filter(p => p.id !== id));
          this._policiesTotalCount.update(n => Math.max(0, n - 1));
          this.toast.success('Policy deleted.');
          onSuccess?.();
        },
        error: err => {
          // Surface the actual server reason so a 405 / 403 is obvious to the user.
          const status = err?.status ? ` (HTTP ${err.status})` : '';
          const detail = err?.error?.message ? ` — ${err.error.message}` : '';
          this.toast.error(`Failed to delete policy${status}${detail}`);
          onError?.();
        }
      });
  }

  // ── Compliance Records ───────────────────────────────────────────────────────
  // _records holds the full list — used by the openCount badge and the
  // issuesPerProgram / issuesPerActivity computeds in the wellness pages.
  // _recordsPage holds only the current page for the records table.
  private _records           = signal<ComplianceRecord[]>([]);
  private _recordsPage       = signal<ComplianceRecord[]>([]);
  private _recordsTotalCount = signal(0);
  private _recordsLoading    = signal(false);
  private _recordsLoadInFlight  = false;
  private _recordsPagedInFlight = false;
  readonly records           = this._records.asReadonly();
  readonly recordsPage       = this._recordsPage.asReadonly();
  readonly recordsTotalCount = this._recordsTotalCount.asReadonly();
  readonly recordsLoading    = this._recordsLoading.asReadonly();
  readonly openCount         = computed(() =>
    this._records().filter(r => r.status !== 'Closed').length
  );

  loadRecords(): void {
    if (this._recordsLoadInFlight) return;
    this._recordsLoadInFlight = true;
    this._recordsLoading.set(true);
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this.http.get<ApiResponse<PagedResult<ComplianceRecord>>>(`${this.base}/compliance-records`, { params })
      .pipe(finalize(() => {
        this._recordsLoading.set(false);
        this._recordsLoadInFlight = false;
      }))
      .subscribe({
        next: res => this._records.set(res.data?.items ?? []),
        error: () => this.toast.error('Failed to load compliance records.')
      });
  }

  /** Server-side paginated records — fills _recordsPage + _recordsTotalCount. */
  loadPagedRecords(page: number, pageSize: number, status?: string): void {
    if (this._recordsPagedInFlight) return;
    this._recordsPagedInFlight = true;
    this._recordsLoading.set(true);
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    if (status && status !== 'All') params = params.set('status', status);
    this.http.get<ApiResponse<PagedResult<ComplianceRecord>>>(`${this.base}/compliance-records`, { params })
      .pipe(finalize(() => {
        this._recordsLoading.set(false);
        this._recordsPagedInFlight = false;
      }))
      .subscribe({
        next: res => {
          if (res.data) {
            this._recordsPage.set(res.data.items);
            this._recordsTotalCount.set(res.data.totalCount);
          }
        },
        error: () => this.toast.error('Failed to load compliance records.')
      });
  }

  createRecord(dto: CreateComplianceRecordDto, onSuccess?: () => void): void {
    this._recordsLoading.set(true);
    this.http.post<ApiResponse<ComplianceRecord>>(`${this.base}/compliance-records`, dto)
      .pipe(finalize(() => this._recordsLoading.set(false)))
      .subscribe({
        next: res => {
          // Update both the full list and the current page view.
          this._records.update(list => [...list, res.data!]);
          this._recordsPage.update(list => [res.data!, ...list]);
          this._recordsTotalCount.update(n => n + 1);
          this.toast.success('Issue reported.');
          onSuccess?.();
        },
        error: () => this.toast.error('Failed to report issue.')
      });
  }

  // Used by HR from programs page — closes regardless of current status
  resolveIssue(id: string, status: string): void {
    const base = `${this.base}/compliance-records/${id}`;
    if (status === 'Reported') {
      this.http.patch<ApiResponse<ComplianceRecord>>(`${base}/start`, {})
        .pipe(switchMap(() => this.http.patch<ApiResponse<ComplianceRecord>>(`${base}/close`, {})))
        .subscribe({
          next: res => { if (res.data) this._patchRecord(res.data); this.toast.success('Issue resolved.'); },
          error: () => this.toast.error('Failed to resolve issue.')
        });
    } else {
      this.closeRecord(id);
    }
  }

  startRecord(id: string): void {
    this.http.patch<ApiResponse<ComplianceRecord>>(`${this.base}/compliance-records/${id}/start`, {})
      .subscribe({
        next: res => { if (res.data) this._patchRecord(res.data); this.toast.success('Record started.'); },
        error: () => this.toast.error('Failed to start record.')
      });
  }

  closeRecord(id: string, resolutionNotes?: string, onSuccess?: () => void): void {
    this.http.patch<ApiResponse<ComplianceRecord>>(
      `${this.base}/compliance-records/${id}/close`,
      { resolutionNotes: resolutionNotes ?? null }
    ).subscribe({
      next:  res => {
        if (res.data) this._patchRecord(res.data);
        this.toast.success('Issue resolved and closed.');
        onSuccess?.();
      },
      error: () => this.toast.error('Failed to close record.')
    });
  }

  private _patchRecord(updated: ComplianceRecord): void {
    // Merge rather than replace — preserve snapshot names (programName,
    // activityName, policyName) from the existing record if the patch
    // response doesn't include them (start/close endpoints may return nulls).
    const merge = (existing: ComplianceRecord): ComplianceRecord => ({
      ...existing,
      ...updated,
      programName:  updated.programName  || existing.programName,
      activityName: updated.activityName || existing.activityName,
      policyName:   updated.policyName   || existing.policyName,
    });
    this._records.update(list => list.map(r => r.id === updated.id ? merge(r) : r));
    this._recordsPage.update(list => list.map(r => r.id === updated.id ? merge(r) : r));
  }

  // ── Audits ───────────────────────────────────────────────────────────────────
  // _audits holds the full list — needed for the draftCount badge in header.
  // _auditsPage holds only the current page for the audits grid.
  private _audits           = signal<Audit[]>([]);
  private _auditsPage       = signal<Audit[]>([]);
  private _auditsTotalCount = signal(0);
  private _auditsLoading    = signal(false);
  private _auditsLoadInFlight  = false;
  private _auditsPagedInFlight = false;
  readonly audits           = this._audits.asReadonly();
  readonly auditsPage       = this._auditsPage.asReadonly();
  readonly auditsTotalCount = this._auditsTotalCount.asReadonly();
  readonly auditsLoading    = this._auditsLoading.asReadonly();
  readonly draftCount       = computed(() => this._audits().filter(a => a.status === 'Draft').length);

  loadAudits(): void {
    if (this._auditsLoadInFlight) return;
    this._auditsLoadInFlight = true;
    this._auditsLoading.set(true);
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this.http.get<ApiResponse<PagedResult<Audit>>>(`${this.base}/audits`, { params })
      .pipe(finalize(() => {
        this._auditsLoading.set(false);
        this._auditsLoadInFlight = false;
      }))
      .subscribe({
        next: res => this._audits.set(res.data?.items ?? []),
        error: () => this.toast.error('Failed to load audits.')
      });
  }

  /** Server-side paginated audits — fills _auditsPage + _auditsTotalCount. */
  loadPagedAudits(page: number, pageSize: number): void {
    if (this._auditsPagedInFlight) return;
    this._auditsPagedInFlight = true;
    this._auditsLoading.set(true);
    const params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    this.http.get<ApiResponse<PagedResult<Audit>>>(`${this.base}/audits`, { params })
      .pipe(finalize(() => {
        this._auditsLoading.set(false);
        this._auditsPagedInFlight = false;
      }))
      .subscribe({
        next: res => {
          if (res.data) {
            this._auditsPage.set(res.data.items);
            this._auditsTotalCount.set(res.data.totalCount);
          }
        },
        error: () => this.toast.error('Failed to load audits.')
      });
  }

  createAudit(dto: CreateAuditDto, onSuccess?: () => void): void {
    this._auditsLoading.set(true);
    this.http.post<ApiResponse<Audit>>(`${this.base}/audits`, dto)
      .pipe(finalize(() => this._auditsLoading.set(false)))
      .subscribe({
        next: res => {
          // Keep both views in sync so the new draft and the draftCount
          // badge update immediately without re-fetching.
          this._audits.update(list => [...list, res.data!]);
          this._auditsPage.update(list => [res.data!, ...list]);
          this._auditsTotalCount.update(n => n + 1);
          this.toast.success('Audit created as draft.');
          onSuccess?.();
        },
        error: () => this.toast.error('Failed to create audit.')
      });
  }

  submitAudit(id: string): void {
    this.http.patch<ApiResponse<Audit>>(`${this.base}/audits/${id}/submit`, {})
      .subscribe({
        next: res => { if (res.data) this._patchAudit(res.data); this.toast.success('Audit submitted.'); },
        error: () => this.toast.error('Failed to submit audit.')
      });
  }

  approveAudit(id: string): void {
    this.http.patch<ApiResponse<Audit>>(`${this.base}/audits/${id}/approve`, {})
      .subscribe({
        next: res => { if (res.data) this._patchAudit(res.data); this.toast.success('Audit approved.'); },
        error: () => this.toast.error('Failed to approve audit.')
      });
  }

  // Rejecting an audit auto-closes it (terminal state). Reason is preserved.
  rejectAudit(id: string, reason: string): void {
    this.http.patch<ApiResponse<Audit>>(`${this.base}/audits/${id}/reject`, { reason })
      .subscribe({
        next: res => { if (res.data) this._patchAudit(res.data); this.toast.success('Audit rejected and closed.'); },
        error: () => this.toast.error('Failed to reject audit.')
      });
  }

  private _patchAudit(updated: Audit): void {
    // Keep both views in sync so workflow transitions (submit/approve/reject)
    // reflect immediately without re-fetching.
    this._audits.update(list => list.map(a => a.id === updated.id ? updated : a));
    this._auditsPage.update(list => list.map(a => a.id === updated.id ? updated : a));
  }
}
