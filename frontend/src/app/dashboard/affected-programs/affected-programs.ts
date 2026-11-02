import { Component, computed, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { ComplianceService } from '../../features/compliance/services/compliance';
import { ComplianceRecord } from '../../features/compliance/models/compliance.model';

interface ProgramGroup {
  programName: string;
  issues: ComplianceRecord[];
}

@Component({
  selector: 'app-affected-programs',
  standalone: true,
  imports: [RouterLink, CommonModule],
  templateUrl: './affected-programs.html',
})
export class AffectedPrograms implements OnInit {
  readonly complianceSvc = inject(ComplianceService);

  private readonly severityOrder: Record<string, number> = { Critical: 0, High: 1, Medium: 2, Low: 3 };

  readonly openIssues = computed((): ComplianceRecord[] =>
    this.complianceSvc.records()
      .filter(r => r.status !== 'Closed')
      .sort((a, b) => (this.severityOrder[a.severity] ?? 9) - (this.severityOrder[b.severity] ?? 9))
  );

  readonly issuesByProgram = computed((): ProgramGroup[] => {
    const map = new Map<string, ProgramGroup>();
    for (const r of this.openIssues()) {
      if (!map.has(r.programId)) map.set(r.programId, { programName: r.programName || r.programId, issues: [] });
      map.get(r.programId)!.issues.push(r);
    }
    return Array.from(map.values());
  });

  ngOnInit(): void {
    if (this.complianceSvc.records().length === 0) this.complianceSvc.loadRecords();
  }

  resolveIssue(id: string, status: string): void {
    this.complianceSvc.resolveIssue(id, status);
  }

  severityColor(s: string): string {
    return ({ Low: 'success', Medium: 'warning', High: 'danger', Critical: 'dark' } as Record<string, string>)[s] ?? 'secondary';
  }

  countBySeverity(issues: ComplianceRecord[], sev: string): number {
    return issues.filter(i => i.severity === sev).length;
  }
}
