import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { AuditLog, AuditLogPagedResult } from '../models/audit-log.model';

@Injectable({ providedIn: 'root' })
export class AuditLogService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  logs       = signal<AuditLog[]>([]);
  totalCount = signal(0);
  totalPages = signal(0);
  page       = signal(1);
  pageSize   = signal(20);

  loadLogs(search?: string): Observable<AuditLogPagedResult> {
    let params = new HttpParams()
      .set('page',     this.page().toString())
      .set('pageSize', this.pageSize().toString());
    if (search) params = params.set('search', search);

    return this.http
      .get<ApiResponse<AuditLogPagedResult>>(`${this.base}/api/v1/audit-logs`, { params })
      .pipe(
        map(res => {
          if (!res.succeeded || !res.data) throw new Error(res.message ?? 'Failed to load logs');
          return res.data;
        }),
        tap(data => {
          this.logs.set(data.items);
          this.totalCount.set(data.totalCount);
          this.totalPages.set(data.totalPages);
        })
      );
  }

  setPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
  }
}