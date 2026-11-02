import { Injectable, inject, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import {
  UserProfile,
  CreateUserRequest,
  UpdateUserRequest,
  PagedResult
} from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserAdminService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  users = signal<UserProfile[]>([]);

  loadUsers(): Observable<UserProfile[]> {
    const params = new HttpParams()
      .set('page', '1')
      .set('pageSize', '1000');

    return this.http
      .get<ApiResponse<PagedResult<UserProfile>>>(`${this.base}/api/v1/users`, { params })
      .pipe(
        map(res => this.unwrap(res).items),
        tap(items => this.users.set(items))
      );
  }

  /**
   * HR-accessible endpoint (GET /api/v1/employees). Returns only Employee-role
   * users. Merges into the same `users` signal so computed maps stay consistent.
   */
  loadEmployees(): Observable<UserProfile[]> {
    const params = new HttpParams()
      .set('page', '1')
      .set('pageSize', '1000');

    return this.http
      .get<ApiResponse<PagedResult<UserProfile>>>(`${this.base}/api/v1/employees`, { params })
      .pipe(
        map(res => this.unwrap(res).items),
        tap(items => this.users.set(items))
      );
  }

  // Dedicated lightweight endpoint that HR can call (the full /users list is Admin-only).
  // Backend already filters to Active Coaches.
  loadActiveCoaches(): Observable<UserProfile[]> {
    return this.http
      .get<ApiResponse<UserProfile[]>>(`${this.base}/api/v1/users/coaches`)
      .pipe(map(res => this.unwrap(res)));
  }

  getById(id: string): Observable<UserProfile> {
    return this.http
      .get<ApiResponse<UserProfile>>(`${this.base}/api/v1/users/${id}`)
      .pipe(map(res => this.unwrap(res)));
  }

  createUser(dto: CreateUserRequest): Observable<UserProfile> {
    return this.http
      .post<ApiResponse<UserProfile>>(`${this.base}/api/v1/users`, dto)
      .pipe(map(res => this.unwrap(res)));
  }

  updateUser(id: string, dto: UpdateUserRequest): Observable<UserProfile> {
    return this.http
      .put<ApiResponse<UserProfile>>(`${this.base}/api/v1/users/${id}`, dto)
      .pipe(map(res => this.unwrap(res)));
  }

  toggleStatus(user: UserProfile): Observable<void> {
    const isActive = user.status === 'Active';
    const url = `${this.base}/api/v1/users/${user.id}/${isActive ? 'deactivate' : 'activate'}`;

    return this.http.patch<ApiResponse<string>>(url, {}).pipe(
      map(res => {
        if (!res.succeeded) throw new Error(res.message ?? 'Operation failed');
      })
    );
  }

  deleteUser(id: string): Observable<void> {
    return this.http.delete<ApiResponse<string>>(`${this.base}/api/v1/users/${id}`).pipe(
      map(res => {
        if (!res.succeeded) throw new Error(res.message ?? 'Delete failed');
      })
    );
  }

  private unwrap<T>(res: ApiResponse<T>): T {
    if (!res.succeeded || res.data == null) {
      throw new Error(res.message ?? 'Operation failed');
    }
    return res.data;
  }
}