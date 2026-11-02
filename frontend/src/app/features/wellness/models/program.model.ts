export interface ProgramDto {
  id: string;
  title: string;
  description: string;
  hrID: string;
  startDate: string;
  endDate: string;
  status: string;
  createdAt: string;
}

export interface CreateProgramDto {
  title: string;
  description: string;
  hrID: string;
  startDate: string;
  endDate: string;
}

export interface UpdateProgramDto {
  title: string;
  description: string;
  startDate: string;
  endDate: string;
}
