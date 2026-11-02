import { HttpClient, HttpParams } from '@angular/common/http';
import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { finalize } from 'rxjs';
import { CreateGoalDto, Goal, UpdateGoalDto, UpdateProgressDto } from '../models/goal.model';
import { AuthService } from '../../../core/services/auth';
import { ToastService } from '../../../shared/services/toast.service';

@Injectable({
  providedIn: 'root',
})
export class GoalService {
  //import url and authService
  private http = inject(HttpClient);
  private auth = inject(AuthService);
  private toast = inject(ToastService);
  private base = `${environment.apiUrl}/api/v1/goals`;

  // private signals
  // _goals = full list (for activeGoals/completedGoals computeds, dashboard usage)
  // _goalsPage = current page for the goals grid display
  private _goals             = signal<Goal[]>([]);
  private _goalsPage         = signal<Goal[]>([]);
  private _goalsTotalCount   = signal(0);
  private _loading           = signal(false);
  private _error             = signal<string | null>(null);
  // Dedupe flags so the dashboard + goals page firing loadAll() / loadPagedGoals()
  // in the same tick don't end up issuing two identical HTTP requests.
  private _loadAllInFlight   = false;
  private _loadPagedInFlight = false;

  // expose as readonly
  readonly goals             = this._goals.asReadonly();
  readonly goalsPage         = this._goalsPage.asReadonly();
  readonly goalsTotalCount   = this._goalsTotalCount.asReadonly();
  readonly loading           = this._loading.asReadonly();
  readonly error             = this._error.asReadonly();
  // computed
  readonly totalCount        = computed(() => this._goals().length);
  readonly activeGoals       = computed(() =>
    this._goals().filter(g => g.status === 'Active')
  );
  readonly completedGoals    = computed(() =>
    this._goals().filter(g => g.status === 'Completed')
  );
  // GET ALL — fetches the full set with a large pageSize so dashboard
  // counts (active/completed) remain accurate even with backend pagination.
  loadAll(): void {
    if (this._loadAllInFlight) return;
    this._loadAllInFlight = true;
    this._loading.set(true);
    this._error.set(null);
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this.http.get<ApiResponse<PagedResult<Goal>>>(this.base, { params })
      .pipe(finalize(() => {
        this._loading.set(false);
        this._loadAllInFlight = false;
      }))
      .subscribe({
        next: res => this._goals.set(res.data?.items ?? []),
        error: () => this._error.set('Failed to load goals')
      });
  }

  /** Server-side paginated goals — fills _goalsPage + _goalsTotalCount. */
  loadPagedGoals(page: number, pageSize: number, status?: string): void {
    if (this._loadPagedInFlight) return;
    this._loadPagedInFlight = true;
    this._loading.set(true);
    this._error.set(null);
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    if (status && status !== 'All') params = params.set('status', status);
    this.http.get<ApiResponse<PagedResult<Goal>>>(this.base, { params })
      .pipe(finalize(() => {
        this._loading.set(false);
        this._loadPagedInFlight = false;
      }))
      .subscribe({
        next: res => {
          if (res.data) {
            this._goalsPage.set(res.data.items);
            this._goalsTotalCount.set(res.data.totalCount);
          }
        },
        error: () => this._error.set('Failed to load goals')
      });
  }

  //CREATE
  create(dto: CreateGoalDto, onSuccess?: () => void): void {
    this._loading.set(true);
    const payload: CreateGoalDto = {
      ...dto,
      employeeId: this.auth.getCurrentUserId()
    };

    this.http.post<ApiResponse<Goal>>(this.base, payload)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: res => {
          // Keep both views in sync — full list for dashboard counts,
          // paged list so the new goal appears immediately.
          this._goals.update(list => [...list, res.data!]);
          this._goalsPage.update(list => [res.data!, ...list]);
          this._goalsTotalCount.update(n => n + 1);
          this.toast.success('Goal created successfully!');
          onSuccess?.();
        },
        error: () => this.toast.error('Failed to create goal.')
      })
  }

  // update progress — backend uses PATCH not PUT
  updateProgress(id: string, dto: UpdateProgressDto): void {
    this._loading.set(true);

    this.http.patch<ApiResponse<Goal>>(`${this.base}/${id}/progress`, dto)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: res => {
          // Backend returns ApiResponse<object> but data shape is still a Goal
          if (res.data) {
            this._goals.update(list =>
              list.map(g => g.id === id ? (res.data as Goal) : g)
            );
            this._goalsPage.update(list =>
              list.map(g => g.id === id ? (res.data as Goal) : g)
            );
          }
          this.toast.success('Progress updated!');
        },
        error: () => this.toast.error('Failed to update progress.')
      });
  }

  extendDeadline(id: string, newEndDate: string): void {
    const goal = this._goals().find(g => g.id === id) ?? this._goalsPage().find(g => g.id === id);
    if (!goal) return;

    const payload: UpdateGoalDto = {
      description:  goal.description,
      targetValue:  goal.targetValue,
      currentValue: goal.currentValue,
      endDate:      newEndDate
    };

    this._loading.set(true);
    this.http.put<ApiResponse<Goal>>(`${this.base}/${id}`, payload)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe({
        next: res => {
          if (res.data) {
            this._goals.update(list =>
              list.map(g => g.id === id ? res.data! : g)
            );
            this._goalsPage.update(list =>
              list.map(g => g.id === id ? res.data! : g)
            );
          }
          this.toast.success('Deadline extended!');
        },
        error: () => this.toast.error('Failed to extend deadline.')
      });
  }

  remove(id: string): void {
    this._loading.set(true);

    this.http.delete<ApiResponse<null>>(`${this.base}/${id}`)
      .pipe(finalize(() => this._loading.set(false)))
      .subscribe(
        {
          next: () => {
            this._goals.update(list => list.filter(x => x.id !== id));
            this._goalsPage.update(list => list.filter(x => x.id !== id));
            this._goalsTotalCount.update(n => Math.max(0, n - 1));
            this.toast.success('Goal deleted.');
          },
          error: () => this.toast.error('Failed to delete goal.')
        }
      )
  }
}



