export interface ProgramReport {
  totalPrograms: number;
  activePrograms: number;
  completedPrograms: number;
  totalParticipants: number;
  averageParticipationRate: number;
}

export interface EngagementReport {
  totalFeedbacks: number;
  averageRating: number;
  averageEngagementScore: number;
  highlyEngagedEmployees: number;
  lowEngagementEmployees: number;
}

export interface ComplianceReport {
  totalAudits: number;
  completedAudits: number;
  pendingAudits: number;
  complianceRate: number;
  issuesIdentified: number;
}

export interface UserDistribution {
  role: string;
  count: number;
  percentage: number;
}

export interface ParticipationTrendReport {
  totalParticipants: number;
  activeParticipants: number;
  participationRate: number;
  topPrograms: ProgramStat[];
  monthlyTrends: MonthlyTrend[];
}

export interface ProgramStat {
  programName: string;
  participants: number;
  completionRate: number;
}

export interface MonthlyTrend {
  month: string;
  participants: number;
  activitiesCompleted: number;
}

export interface EngagementScoreReport {
  averageScore: number;
  highlyEngaged: number;
  moderatelyEngaged: number;
  lowEngaged: number;
  topEmployees: EmployeeScore[];
  trend: EngagementTrend[];
}

export interface EmployeeScore {
  name: string;
  score: number;
}

export interface EngagementTrend {
  label: string;
  score: number;
  activeUsers: number;
}