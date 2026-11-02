import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { ParticipationService } from '../../services/participation';
import { Participation } from '../../models/participation.model';
import { ProgramService } from '../../../wellness/services/program';
import { CommonModule } from '@angular/common';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';

@Component({
  selector: 'app-my-participation',
  imports: [CommonModule, Skeleton],
  templateUrl: './my-participation.html',
  styleUrl: './my-participation.scss',
})
export class MyParticipation implements OnInit {
  protected participation = inject(ParticipationService);
  protected programService = inject(ProgramService);

  programName(record: any): string {
    const id = record.programID ?? record.programId;
    return this.programService.programs().find(p => p.id === id)?.title ?? '';
  }

  filterStatus = signal<string>('All');
  statusTabs   = ['All', 'Active', 'Completed'];

  // ── Pagination (server-side) ──────────────────────────────────────
  // Source of truth: ParticipationService.participationsPage() and
  // participationsTotalCount(), filled by loadPagedMine().
  page     = signal(1);
  pageSize = 6;

  pagedParticipations = computed(() => this.participation.participationsPage());
  totalCount          = computed(() => this.participation.participationsTotalCount());
  totalPages          = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.participation.loadPagedMine(p, this.pageSize, this.filterStatus());
  }
  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  setFilter(status: string): void {
    this.filterStatus.set(status);
    this.page.set(1);
    this.participation.loadPagedMine(1, this.pageSize, status);
  }

  ngOnInit(): void {
    this.participation.loadMine();
    this.participation.loadPagedMine(1, this.pageSize, this.filterStatus());
    this.programService.loadPrograms().subscribe();
  }
}
