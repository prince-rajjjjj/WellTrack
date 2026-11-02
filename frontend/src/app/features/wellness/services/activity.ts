import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize, map, shareReplay, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import {
  ActivityDto, CreateActivityDto, AssignCoachDto,
  AttendanceDto, MarkAttendanceDto,
} from '../models/activity.model';

@Injectable({ providedIn: 'root' })
export class ActivityService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/v1/activities`;

  activities = signal<ActivityDto[]>([]);

  // Tracks an in-flight loadActivities() call per programId so concurrent
  // subscribers (e.g. dashboard + activities page race) share one HTTP request
  // instead of stampeding the backend.
  private _activitiesInFlight = new Map<string, Observable<ActivityDto[]>>();

  loadActivities(programId?: string): Observable<ActivityDto[]> {
    const key = programId ?? '__all__';
    const existing = this._activitiesInFlight.get(key);
    if (existing) return existing;

    let params = new HttpParams();
    if (programId) params = params.set('programId', programId);
    // Backend now returns PagedResult by default. Pass a large pageSize to get
    // everything for non-paginated consumers (compliance enrich, feedback, etc.).
    params = params.set('page', '1').set('pageSize', '1000');

    const request$ = this.http.get<ApiResponse<PagedResult<ActivityDto>>>(this.base, { params }).pipe(
      map(res => this.unwrap(res).items),
      tap(items => this.activities.set(items)),
      finalize(() => this._activitiesInFlight.delete(key)),
      shareReplay({ bufferSize: 1, refCount: false }),
    );
    this._activitiesInFlight.set(key, request$);
    return request$;
  }

  /** Server-side paginated activities with full filter support. */
  loadPagedActivities(
    page: number, pageSize: number,
    programId?: string, search?: string,
    mode?: string, status?: string,
    view?: string,
  ): Observable<PagedResult<ActivityDto>> {
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    if (programId) params = params.set('programId', programId);
    if (search)    params = params.set('search', search);
    if (mode)      params = params.set('mode', mode);
    if (status)    params = params.set('status', status);
    if (view)      params = params.set('view', view);

    return this.http.get<ApiResponse<PagedResult<ActivityDto>>>(this.base, { params }).pipe(
      map(res => this.unwrap(res))
    );
  }

  getById(id: string): Observable<ActivityDto> {
    return this.http.get<ApiResponse<ActivityDto>>(`${this.base}/${id}`).pipe(
      map(res => this.unwrap(res))
    );
  }

  create(dto: CreateActivityDto): Observable<ActivityDto> {
    return this.http.post<ApiResponse<ActivityDto>>(this.base, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  update(id: string, dto: CreateActivityDto): Observable<ActivityDto> {
    return this.http.put<ApiResponse<ActivityDto>>(`${this.base}/${id}`, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  assignCoach(id: string, dto: AssignCoachDto): Observable<ActivityDto> {
    return this.http.patch<ApiResponse<ActivityDto>>(`${this.base}/${id}/assign-coach`, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  start(id: string): Observable<ActivityDto> {
    return this.http.post<ApiResponse<ActivityDto>>(`${this.base}/${id}/start`, {}).pipe(
      map(res => this.unwrap(res))
    );
  }

  complete(id: string): Observable<ActivityDto> {
    return this.http.post<ApiResponse<ActivityDto>>(`${this.base}/${id}/complete`, {}).pipe(
      map(res => this.unwrap(res))
    );
  }

  cancel(id: string): Observable<ActivityDto> {
    return this.http.post<ApiResponse<ActivityDto>>(`${this.base}/${id}/cancel`, {}).pipe(
      map(res => this.unwrap(res))
    );
  }

  // Employee self-enrolls
  join(id: string): Observable<AttendanceDto> {
    return this.http.post<ApiResponse<AttendanceDto>>(`${this.base}/${id}/join`, {}).pipe(
      map(res => this.unwrap(res))
    );
  }

  // Employee unenrolls
  leave(id: string): Observable<void> {
    return this.http.delete<ApiResponse<unknown>>(`${this.base}/${id}/join`).pipe(
      map(res => { if (!res.succeeded) throw new Error(res.message ?? 'Failed to leave'); })
    );
  }

  // Employee marks themselves as watched (online)
  markWatched(id: string): Observable<AttendanceDto> {
    return this.http.post<ApiResponse<AttendanceDto>>(`${this.base}/${id}/watch`, {}).pipe(
      map(res => this.unwrap(res))
    );
  }

  // Attendance (coach view)
  getAttendance(activityId: string): Observable<AttendanceDto[]> {
    return this.http.get<ApiResponse<AttendanceDto[]>>(`${this.base}/${activityId}/attendance`).pipe(
      map(res => this.unwrap(res))
    );
  }

  markAttendance(activityId: string, dto: MarkAttendanceDto): Observable<AttendanceDto> {
    return this.http.post<ApiResponse<AttendanceDto>>(`${this.base}/${activityId}/attendance`, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  finalizeAttendance(activityId: string, entries: { employeeID: string; isPresent: boolean }[]): Observable<ActivityDto> {
    return this.http.post<ApiResponse<ActivityDto>>(`${this.base}/${activityId}/attendance/finalize`, { entries }).pipe(
      map(res => this.unwrap(res))
    );
  }

  private unwrap<T>(res: ApiResponse<T>): T {
    if (!res.succeeded || res.data == null) throw new Error(res.message ?? 'Operation failed');
    return res.data;
  }
}
