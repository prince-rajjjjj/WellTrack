// ── Matches GoalDto.cs ──────────────────────────────────────────
// What the backend SENDS to us (GET response)
export interface Goal {
  id:                 string;
  employeeId:         string;
  description:        string;
  targetValue:        number;
  currentValue:       number;
  progressPercentage: number;   // backend computes this — never send it back
  startDate:          string;   // "2026-01-15T00:00:00" — ISO format
  endDate:            string;
  status:             string;   // 'Active' | 'Pending' | 'Completed' etc.
  createdAt:          string;
  updatedAt:          string;
}

// ── Matches CreateGoalDto.cs ────────────────────────────────────
// What we SEND to backend (POST body)
export interface CreateGoalDto {
  employeeId:  string;   // comes from JWT — not from form input
  description: string;
  targetValue: number;
  startDate:   string;
  endDate:     string;
}

// ── Matches UpdateGoalDto.cs ────────────────────────────────────
// What we SEND to backend (PUT body)
export interface UpdateGoalDto {
  description:  string;
  targetValue:  number;
  currentValue: number;
  endDate:      string;
}

// ── For progress update endpoint ─────────────────────────────────
// PATCH /api/v1/goals/:id/progress
// Backend property: CurrentValue (decimal) — .NET serializes as camelCase so currentValue matches
export interface UpdateProgressDto {
  currentValue: number;
}