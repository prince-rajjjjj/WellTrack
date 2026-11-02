import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import {
  EmployeeEngagementDto,
  HrEngagementDto,
  OrgSummaryDto,
  OverallLeaderboardEntry,
  MostImprovedEntry,
  FactorHealthDto,
  ProgramHealthDto,
  RecalculateEngagementDto,
} from '../models/engagement.model';
 
@Injectable({ providedIn: 'root' })
export class EngagementService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/v1/engagement`;
 
  // ── Employee ─────────────────────────────────────────────────────────
  getMyEngagement(): Observable<ApiResponse<EmployeeEngagementDto[]>> {
    return this.http.get<ApiResponse<EmployeeEngagementDto[]>>(`${this.base}/my`);
  }
 
  // ── HR Dashboard analytics ────────────────────────────────────────────
  getOrgSummary(): Observable<ApiResponse<OrgSummaryDto>> {
    return this.http.get<ApiResponse<OrgSummaryDto>>(`${this.base}/summary/overall`);
  }
 
  getOverallLeaderboard(top = 10): Observable<ApiResponse<OverallLeaderboardEntry[]>> {
    return this.http.get<ApiResponse<OverallLeaderboardEntry[]>>(
      `${this.base}/leaderboard/overall?top=${top}`);
  }
 
  getMostImproved(top = 10): Observable<ApiResponse<MostImprovedEntry[]>> {
    return this.http.get<ApiResponse<MostImprovedEntry[]>>(
      `${this.base}/leaderboard/most-improved?top=${top}`);
  }
 
  getFactorHealth(): Observable<ApiResponse<FactorHealthDto>> {
    return this.http.get<ApiResponse<FactorHealthDto>>(`${this.base}/factor-health`);
  }
 
  getProgramHealth(): Observable<ApiResponse<ProgramHealthDto[]>> {
    return this.http.get<ApiResponse<ProgramHealthDto[]>>(`${this.base}/program-health`);
  }
 
  // ── Recalculate ───────────────────────────────────────────────────────
  /** Single row recalculate — per-row ↻ button */
  recalculate(dto: RecalculateEngagementDto): Observable<ApiResponse<HrEngagementDto>> {
    return this.http.post<ApiResponse<HrEngagementDto>>(`${this.base}/recalculate`, dto);
  }
 
  /** Bulk sync — "Sync Scores" button. Returns { updated: N } */
  recalculateAll(): Observable<ApiResponse<{ updated: number }>> {
    return this.http.post<ApiResponse<{ updated: number }>>(`${this.base}/recalculate/all`, {});
  }
 
}

 
 