export interface Participation {
  id: string;
  programId: string;
  employeeId: string;
  activityId: string | null;
  activityTitle: string | null;
  date: string;
  status: string;
  notes: string | null;
}

export interface CreateParticipationDto {
  programId: string;
  employeeId: string;
  activityId: string | null;
  date: string;
  notes?: string;
}