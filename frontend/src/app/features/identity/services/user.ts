import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import { UserProfile, UpdateProfileRequest, ChangePasswordRequest } from '../models/user.model';

@Injectable({ providedIn: 'root' })
export class UserService {
  private http = inject(HttpClient);
  private base = environment.apiUrl;

  getMyProfile(): Observable<ApiResponse<UserProfile>> {
    return this.http.get<ApiResponse<UserProfile>>(`${this.base}/api/v1/auth/profile`);
  }

  updateProfile(dto: UpdateProfileRequest): Observable<ApiResponse<UserProfile>> {
    return this.http.patch<ApiResponse<UserProfile>>(`${this.base}/api/v1/users/me`, dto);
  }

  changePassword(dto: ChangePasswordRequest): Observable<ApiResponse<string>> {
    return this.http.put<ApiResponse<string>>(`${this.base}/api/v1/auth/change-password`, dto);
  }
}