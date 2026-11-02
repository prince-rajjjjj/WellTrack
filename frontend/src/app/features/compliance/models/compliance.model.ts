export interface Policy {
  id: string;
  policyName: string;
  description: string;
  category: string;
  severity: string;
  status: string;
  createdDate: string;
  createdAt: string;
}

export interface CreatePolicyDto {
  policyName: string;
  description: string;
  category: string;
  severity: string;
}

export interface ComplianceRecord {
  id: string;
  programId: string;
  programName: string;
  activityId: string | null;
  activityName: string;
  policyId: string;
  policyName: string;
  issueType: string;
  severity: string;
  notes: string;
  status: string;
  assignedTo: string | null;
  assignedToId: string | null;
  resolutionNotes: string | null;
  // User id (string Guid) of whoever reported the issue.
  // Used so a CO can see "issues I personally reported".
  createdBy: string | null;
  createdDate: string;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateComplianceRecordDto {
  programId: string;
  activityId: string | null;
  policyId: string;
  // Snapshots — sent so the record preserves human-readable context even if
  // the program / activity is later deleted in the Wellness service.
  programName?: string;
  activityName?: string | null;
  issueType: string;
  severity: string;
  notes: string;
}

// Status values: 'Reported' | 'InProgress' | 'Closed'

export interface Audit {
  id: string;
  complianceOfficerId: string;
  scope: string;
  findings: string | null;
  rejectionReason: string | null;
  date: string;
  status: string;
}

export interface CreateAuditDto {
  complianceOfficerId: string;
  scope: string;
  findings?: string;
}
