import { computed, inject, Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { finalize, Subject, switchMap } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { Participation } from '../models/participation.model';
import { AuthService } from '../../../core/services/auth';
import { ToastService } from '../../../shared/services/toast.service';

@Injectable({ providedIn: 'root' })
export class ParticipationService {
  private http  = inject(HttpClient);
  private auth  = inject(AuthService);
  private toast = inject(ToastService);
  private base  = `${environment.apiUrl}/api/v1/participation`;

  // _participations = full list for dashboard counts (active/completed/total).
  // _participationsPage = current page for the my-participation grid.
  private _participations           = signal<Participation[]>([]);
  private _participationsPage       = signal<Participation[]>([]);
  private _participationsTotalCount = signal(0);
  private _loading                  = signal(false);
  private _error                    = signal<string | null>(null);
  // Dedupe flags for loadMine / loadAll only.
  private _loadMineInFlight  = false;
  private _loadAllInFlight   = false;
  // switchMap subject — ensures each filter/page change cancels the previous
  // paged request so stale responses can never overwrite newer ones.
  private _pagedTrigger$ = new Subject<{ page: number; pageSize: number; status?: string }>();

  readonly participations           = this._participations.asReadonly();
  readonly participationsPage       = this._participationsPage.asReadonly();
  readonly participationsTotalCount = this._participationsTotalCount.asReadonly();
  readonly loading                  = this._loading.asReadonly();
  readonly error                    = this._error.asReadonly();

  readonly totalCount     = computed(() => this._participations().length);
  readonly completedCount = computed(() =>
    this._participations().filter(p => p.status === 'Completed').length
  );
  readonly activeCount    = computed(() =>
    this._participations().filter(p => p.status === 'Active').length
  );

  constructor() {
    // switchMap cancels the previous in-flight paged request whenever a new
    // filter/page is requested, so stale responses never overwrite fresh ones.
    this._pagedTrigger$.pipe(
      switchMap(({ page, pageSize, status }) => {
        this._loading.set(true);
        this._error.set(null);
        let params = new HttpParams()
          .set('page', String(page))
          .set('pageSize', String(pageSize));
        if (status && status !== 'All') params = params.set('status', status);
        return this.http
          .get<ApiResponse<PagedResult<Participation>>>(`${this.base}/my`, { params })
          .pipe(finalize(() => this._loading.set(false)));
      })
    ).subscribe({
      next: res => {
        if (res.data) {
          this._participationsPage.set(res.data.items);
          this._participationsTotalCount.set(res.data.totalCount);
        }
      },
      error: () => this._error.set('Failed to load participations')
    });
  }

  loadMine(): void {
    if (this._loadMineInFlight) return;
    this._loadMineInFlight = true;
    this._loading.set(true);
    this._error.set(null);

    // Backend now returns PagedResult; pull a large page to populate the
    // full list used by the dashboard counts.
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this.http.get<ApiResponse<PagedResult<Participation>>>(`${this.base}/my`, { params })
      .pipe(finalize(() => {
        this._loading.set(false);
        this._loadMineInFlight = false;
      }))
      .subscribe({
        next: res => this._participations.set(res.data?.items ?? []),
        error: () => this._error.set('Failed to load participations')
      });
  }

  /** Server-side paginated mine — fills _participationsPage + _participationsTotalCount. */
  loadPagedMine(page: number, pageSize: number, status?: string): void {
    this._pagedTrigger$.next({ page, pageSize, status });
  }

  loadAll(): void {
    if (this._loadAllInFlight) return;
    this._loadAllInFlight = true;
    this._loading.set(true);
    this._error.set(null);

    // GET /participation (without /my) still returns IEnumerable<T>; no pagination there.
    this.http.get<ApiResponse<Participation[]>>(this.base)
      .pipe(finalize(() => {
        this._loading.set(false);
        this._loadAllInFlight = false;
      }))
      .subscribe({
        next: res => this._participations.set(res.data ?? []),
        error: () => this._error.set('Failed to load participations')
      });
  }

  joinActivity(programId: string, activityId: string, activityTitle: string): void {
    const payload = { programId, activityId, activityTitle };

    this.http.post<ApiResponse<{ participationId: string }>>(`${this.base}/join-activity`, payload)
      .subscribe({
        next: () => this.loadMine(),
        error: () => this.toast.error('Failed to record participation.')
      });
  }
}
