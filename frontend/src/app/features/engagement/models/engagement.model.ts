// ── Employee personal view ────────────────────────────────────────────────
export interface EmployeeEngagementDto {
  programID:        string;
  totalScore:       number;
  previousScore?:   number | null;
  engagementLevel:  string;
  breakdown:        ScoreBreakdownDto;
  improvementTips:  string[];
  lastCalculatedAt: string;
}

export interface ScoreBreakdownDto {
  attendanceScore:  number;   // max 30
  activityScore:    number;   // max 25
  goalScore:        number;   // max 25
  feedbackScore:    number;   // max 10
  consistencyScore: number;   // max 10
}

// ── HR raw records ────────────────────────────────────────────────────────
export interface HrEngagementDto {
  id:               string;
  employeeID:       string;
  programID:        string;
  totalScore:       number;
  previousScore?:   number | null;
  engagementLevel:  string;
  attendanceScore:  number;
  activityScore:    number;
  goalScore:        number;
  feedbackScore:    number;
  consistencyScore: number;
  lastCalculatedAt: string;
}

// ── HR Dashboard analytics ────────────────────────────────────────────────
export interface OrgSummaryDto {
  totalTracked:    number;
  orgAverageScore: number;
  highCount:       number;
  mediumCount:     number;
  lowCount:        number;
  percentHigh:     number;
  percentAtRisk:   number;
}

export interface OverallLeaderboardEntry {
  rank:            number;
  employeeId:      string;
  avgScore:        number;
  programCount:    number;
  engagementLevel: string;
  scoreDelta?:     number | null;
}

export interface MostImprovedEntry {
  rank:            number;
  employeeId:      string;
  currentScore:    number;
  previousScore:   number;
  delta:           number;
  engagementLevel: string;
}

export interface FactorHealthDto {
  attendanceAvg:  number;
  activityAvg:    number;
  goalAvg:        number;
  feedbackAvg:    number;
  consistencyAvg: number;
  attendancePct:  number;
  activityPct:    number;
  goalPct:        number;
  feedbackPct:    number;
  consistencyPct: number;
}

export interface ProgramHealthDto {
  programId:      string;
  totalEmployees: number;
  avgScore:       number;
  highCount:      number;
  mediumCount:    number;
  lowCount:       number;
  trend?:         number | null;
}

// ── Legacy (kept for employee chart, per-program endpoints) ───────────────
export interface EngagementSummaryDto {
  programID:        string;
  totalEmployees:   number;
  highEngagement:   number;
  mediumEngagement: number;
  lowEngagement:    number;
  averageScore:     number;
}

export interface RecalculateEngagementDto {
  employeeId: string;
  programId:  string;
}
