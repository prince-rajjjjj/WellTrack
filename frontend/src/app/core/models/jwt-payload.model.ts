// Matches your .NET AuthService claims — handles both short and ASP.NET Core long-form names
export interface JwtPayload {
  sub?: string;
  exp: number;
  name?: string;
  role?: string;

  // ASP.NET Core long-form claim names
  'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'?: string;
  'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'?: string;
}