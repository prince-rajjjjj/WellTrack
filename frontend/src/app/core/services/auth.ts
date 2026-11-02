import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { jwtDecode } from 'jwt-decode';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { JwtPayload } from '../models/jwt-payload.model';
import { LoginRequest, LoginResponse } from '../../features/identity/models/login.model';
import { NotificationService } from '../../features/notification/services/notification';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly TOKEN_KEY = 'welltrack_token';
  private readonly NAME_KEY  = 'welltrack_name';
  private readonly http      = inject(HttpClient);
  private readonly router    = inject(Router);
  private notificationService = inject(NotificationService);

  private _name = signal<string>(this.loadName());
  name = this._name.asReadonly();

  
  private loadName(): string {
    const cached = localStorage.getItem(this.NAME_KEY);
    if (cached) return cached;

    const decoded = this.decode();
    return decoded?.name
        ?? decoded?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']
        ?? 'User';
  }

  /** Called after profile save / login — persists to cache too. */
  setName(name: string): void {
    this._name.set(name);
    localStorage.setItem(this.NAME_KEY, name);
  }

  // ─── Auth actions ────────────────────────────────────────────────
  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http
      .post<ApiResponse<LoginResponse>>(`${environment.apiUrl}/api/v1/auth/login`, credentials)
      .pipe(
        tap(res => {
          if (res.succeeded && res.data?.token) {
            localStorage.setItem(this.TOKEN_KEY, res.data.token);
            this.setName(res.data.name);                   // persist + signal
            this.notificationService.startPolling();
            this.notificationService.loadAll().subscribe();
          }
        })
      );
  }

  logout(): void {
    this.http.post(`${environment.apiUrl}/api/v1/auth/logout`, {})
      .subscribe({ next: () => this.finishLogout(), error: () => this.finishLogout() });
  }

  private finishLogout(): void {
    this.notificationService.stopPolling();
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.NAME_KEY);              // clear the name cache too
    this._name.set('User');
    this.router.navigate(['/login']);
  }

  // ─── Token ───────────────────────────────────────────────────────
  getToken(): string | null { return localStorage.getItem(this.TOKEN_KEY); }

  isLoggedIn(): boolean {
    const token = this.getToken();
    if (!token) return false;
    try {
      const { exp } = jwtDecode<JwtPayload>(token);
      if (exp * 1000 <= Date.now()) {
        localStorage.removeItem(this.TOKEN_KEY);
        localStorage.removeItem(this.NAME_KEY);
        return false;
      }
      return true;
    } catch {
      localStorage.removeItem(this.TOKEN_KEY);
      localStorage.removeItem(this.NAME_KEY);
      return false;
    }
  }

  // ─── JWT-derived getters (id + role only — they don't change at runtime) ────
  private decode(): JwtPayload | null {
    const token = this.getToken();
    if (!token) return null;
    try { return jwtDecode<JwtPayload>(token); }
    catch { return null; }
  }

  getCurrentUserId(): string {
    const d = this.decode();
    return d?.sub
        ?? d?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier']
        ?? '';
  }

  getCurrentRole(): string {
    const d = this.decode();
    return d?.role
        ?? d?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
        ?? '';
  }

  getCurrentName(): string { return this._name(); }

  // ─── Role helpers ────────────────────────────────────────────────
  isHROrAdmin(): boolean { const r = this.getCurrentRole(); return r === 'HR' || r === 'Admin'; }
  isCoach():    boolean  { return this.getCurrentRole() === 'Coach';    }
  isEmployee(): boolean  { return this.getCurrentRole() === 'Employee'; }
  isAdmin():    boolean  { return this.getCurrentRole() === 'Admin';    }
}