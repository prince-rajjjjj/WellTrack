export interface UserProfile {
  id: string;
  name: string;
  email: string;
  role: string;
  phone?: string;
  status: string;
  createdAt: string;
}

export interface UpdateProfileRequest {
  name: string;
  phone: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
}

// ─── Admin user management ───────────────────────────────
export enum UserRole {
  Admin = 1,
  HR = 2,
  Employee = 3,
  Coach = 4,
  ComplianceOfficer = 5
}

export enum UserStatus {
  Active = 1,
  Inactive = 2,
  Pending = 3,
  Archived = 6
}

export interface CreateUserRequest {
  name: string;
  email: string;
  phone: string;
  role: UserRole;
  password: string;
}

export interface UpdateUserRequest {
  name: string;
  phone: string;
  role: UserRole;
  status: UserStatus;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}