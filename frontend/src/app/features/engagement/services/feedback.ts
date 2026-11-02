import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import {
  FeedbackDto,
  CreateFeedbackDto,
  UpdateFeedbackDto,
} from '../models/feedback.model';

@Injectable({ providedIn: 'root' })
export class FeedbackService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/v1/feedback`;

  // ── Employee ──────────────────────────────────────────────────────────
  /** All feedback the logged-in employee has ever submitted.
   *  Backend now returns PagedResult; we pull a large page and unwrap it
   *  to keep the existing array contract for consumers. */
  getMyFeedback(): Observable<ApiResponse<FeedbackDto[]>> {
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    return this.http.get<ApiResponse<PagedResult<FeedbackDto>>>(`${this.base}/my`, { params }).pipe(
      map(res => ({
        succeeded: res.succeeded,
        message:   res.message,
        data:      res.data ? res.data.items : null,
        errors:    res.errors,
      } as ApiResponse<FeedbackDto[]>))
    );
  }

  /** Submit feedback — one per employee per program */
  create(dto: CreateFeedbackDto): Observable<ApiResponse<FeedbackDto>> {
    return this.http.post<ApiResponse<FeedbackDto>>(this.base, dto);
  }

  /** Edit own feedback */
  update(id: string, dto: UpdateFeedbackDto): Observable<ApiResponse<FeedbackDto>> {
    return this.http.put<ApiResponse<FeedbackDto>>(`${this.base}/${id}`, dto);
  }

  /** Delete own feedback */
  delete(id: string): Observable<ApiResponse<object>> {
    return this.http.delete<ApiResponse<object>>(`${this.base}/${id}`);
  }

  // ── HR / Admin / Coach ────────────────────────────────────────────────
  /** All feedback (HR/Admin). Backend returns PagedResult; unwrap to array. */
  getAllFeedback(): Observable<ApiResponse<FeedbackDto[]>> {
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    return this.http.get<ApiResponse<PagedResult<FeedbackDto>>>(this.base, { params }).pipe(
      map(res => ({
        succeeded: res.succeeded,
        message:   res.message,
        data:      res.data ? res.data.items : null,
        errors:    res.errors,
      } as ApiResponse<FeedbackDto[]>))
    );
  }

  /** All feedback submitted for a specific program */
  getByProgram(programId: string): Observable<ApiResponse<FeedbackDto[]>> {
    return this.http.get<ApiResponse<FeedbackDto[]>>(
      `${this.base}/program/${programId}`
    );
  }

}
