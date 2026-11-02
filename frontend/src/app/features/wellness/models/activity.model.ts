export type ActivityMode = 'Offline' | 'Online';

export interface ActivityDto {
  id: string;
  programID: string;
  programName: string;
  title: string;
  description: string;
  type: string;

  date: string;
  duration: number;
  maxParticipants: number;

  mode: ActivityMode | string;
  location?: string | null;
  videoUrl?: string | null;

  status: string | number;
  assignedCoachID?: string | null;
  assignedCoachName?: string | null;
  attendanceFinalized: boolean;

  enrolledCount: number;
  attendedCount: number;

  // Per-current-user state
  isEnrolled: boolean;
  hasAttended: boolean;

  createdAt: string;
}

export interface CreateActivityDto {
  programID: string;
  title: string;
  description: string;
  type: string;
  date: string;
  duration: number;
  maxParticipants: number;
  mode: ActivityMode;
  location?: string;
  videoUrl?: string;
  assignedCoachID?: string;
  assignedCoachName?: string;
}

export interface AssignCoachDto {
  coachID: string;
  coachName: string;
}

export interface AttendanceDto {
  id: string;
  activityID: string;
  employeeID: string;
  employeeName: string;
  isPresent: boolean;
  joinTime?: string;
  leaveTime?: string;
  notes?: string;
}

export interface MarkAttendanceDto {
  employeeID: string;
  employeeName: string;
  isPresent: boolean;
  joinTime?: string;
  leaveTime?: string;
  notes?: string;
}

