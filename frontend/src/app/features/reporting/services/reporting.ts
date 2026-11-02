import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { ApiResponse } from '../../../core/models/api-response.model';
import {
  ProgramReport, EngagementReport, ComplianceReport,
  UserDistribution, ParticipationTrendReport, EngagementScoreReport
} from '../models/report.model';

@Injectable({ providedIn: 'root' })
export class ReportingService {
  private http = inject(HttpClient);
  private base = `${environment.apiUrl}/api/v1/reports`;

  getPrograms(): Observable<ProgramReport> {
    return this.http.get<ApiResponse<ProgramReport>>(`${this.base}/programs`)
      .pipe(map(r => r.data!));
  }

  getEngagement(): Observable<EngagementReport> {
    return this.http.get<ApiResponse<EngagementReport>>(`${this.base}/engagement`)
      .pipe(map(r => r.data!));
  }

  getCompliance(): Observable<ComplianceReport> {
    return this.http.get<ApiResponse<ComplianceReport>>(`${this.base}/compliance`)
      .pipe(map(r => r.data!));
  }

  getUserDistribution(): Observable<UserDistribution[]> {
    return this.http.get<ApiResponse<UserDistribution[]>>(`${this.base}/user-distribution`)
      .pipe(map(r => r.data!));
  }

  getParticipationTrends(): Observable<ParticipationTrendReport> {
    return this.http.get<ApiResponse<ParticipationTrendReport>>(`${this.base}/participation-trends`)
      .pipe(map(r => r.data!));
  }

  getEngagementScores(): Observable<EngagementScoreReport> {
    return this.http.get<ApiResponse<EngagementScoreReport>>(`${this.base}/engagement-scores`)
      .pipe(map(r => r.data!));
  }
}