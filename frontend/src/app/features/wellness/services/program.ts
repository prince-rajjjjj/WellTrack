import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { finalize, map, shareReplay, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse, PagedResult } from '../../../core/models/api-response.model';
import { ProgramDto, CreateProgramDto, UpdateProgramDto } from '../models/program.model';

@Injectable({ providedIn: 'root' })
export class ProgramService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/v1/programs`;

  programs = signal<ProgramDto[]>([]);

  // Concurrent loadPrograms() calls (dashboard + programs page + activities
  // dropdown all firing at once) share one HTTP request via shareReplay.
  // Cleared on completion so subsequent calls fetch fresh data.
  private _programsInFlight: Observable<ProgramDto[]> | null = null;

  loadPrograms(): Observable<ProgramDto[]> {
    if (this._programsInFlight) return this._programsInFlight;

    // Backend returns PagedResult; request a large pageSize for the
    // "give me everything" use case (dropdowns, lookups, name maps).
    const params = new HttpParams().set('page', '1').set('pageSize', '10000');
    this._programsInFlight = this.http.get<ApiResponse<PagedResult<ProgramDto>>>(this.base, { params }).pipe(
      map(res => this.unwrap(res).items),
      tap(items => this.programs.set(items)),
      finalize(() => { this._programsInFlight = null; }),
      shareReplay({ bufferSize: 1, refCount: false }),
    );
    return this._programsInFlight;
  }

  /** Server-side paginated programs with optional search/status filter. */
  loadPagedPrograms(
    page: number, pageSize: number,
    search?: string, status?: string,
  ): Observable<PagedResult<ProgramDto>> {
    let params = new HttpParams()
      .set('page', String(page))
      .set('pageSize', String(pageSize));
    if (search) params = params.set('search', search);
    if (status) params = params.set('status', status);

    return this.http.get<ApiResponse<PagedResult<ProgramDto>>>(this.base, { params }).pipe(
      map(res => this.unwrap(res))
    );
  }

  getById(id: string): Observable<ProgramDto> {
    return this.http.get<ApiResponse<ProgramDto>>(`${this.base}/${id}`).pipe(
      map(res => this.unwrap(res))
    );
  }

  create(dto: CreateProgramDto): Observable<ProgramDto> {
    return this.http.post<ApiResponse<ProgramDto>>(this.base, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  update(id: string, dto: UpdateProgramDto): Observable<ProgramDto> {
    return this.http.put<ApiResponse<ProgramDto>>(`${this.base}/${id}`, dto).pipe(
      map(res => this.unwrap(res))
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/${id}`).pipe(
      map(res => { if (!res.succeeded) throw new Error(res.message ?? 'Delete failed'); })
    );
  }

  private unwrap<T>(res: ApiResponse<T>): T {
    if (!res.succeeded || res.data == null) throw new Error(res.message ?? 'Operation failed');
    return res.data;
  }
}
