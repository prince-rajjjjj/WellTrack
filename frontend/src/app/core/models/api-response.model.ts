export interface ApiResponse<T> {
  succeeded: boolean;
  message: string | null;
  data: T | null;
  errors: { [field: string]: string[] } | null;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}
