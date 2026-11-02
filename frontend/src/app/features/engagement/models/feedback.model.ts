// ── Mirrors backend FeedbackDto ───────────────────────────────────────────
export interface FeedbackDto {
  id: string;
  programID: string;
  activityID?: string | null;
  activityTitle?: string | null;
  employeeID: string;
  rating: number;
  comments: string | null;
  date: string;
}

// ── Mirrors backend CreateFeedbackDto ─────────────────────────────────────
export interface CreateFeedbackDto {
  programId: string;
  activityId?: string;
  activityTitle?: string;
  rating: number;
  comments?: string;
}

// ── Mirrors backend UpdateFeedbackDto ─────────────────────────────────────
export interface UpdateFeedbackDto {
  rating: number;
  comments?: string | null;
}

// ── Mirrors backend FeedbackStatsDto ──────────────────────────────────────
export interface FeedbackStatsDto {
  programID: string;
  totalFeedbacks: number;
  averageRating: number;
  fiveStarCount: number;
  fourStarCount: number;
  threeStarCount: number;
  twoStarCount: number;
  oneStarCount: number;
}
