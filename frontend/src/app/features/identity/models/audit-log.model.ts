export interface AuditLog {
  id: string;
  userId: string;
  action: string;
  resource: string;
  timestamp: string;
  details?: string;
}

export interface AuditLogPagedResult {
  items: AuditLog[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}