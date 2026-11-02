import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { GoalService } from '../../services/goal';
import { Goal } from '../../models/goal.model';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GoalCard } from '../../components/goal-card/goal-card';
import { ConfirmModal } from '../../../../shared/components/confirm-modal/confirm-modal';
import { Skeleton } from '../../../../shared/components/skeleton/skeleton';

@Component({
  selector: 'app-goals',
  standalone: true,
  imports: [RouterLink,CommonModule,GoalCard,ConfirmModal,Skeleton],
  templateUrl: './goals.html',
  styleUrl: './goals.scss',
})
export class Goals implements OnInit {
  readonly goalService = inject(GoalService);

  showConfirm = false;
  pendingDeleteId = '';
  filterStatus = signal('All');

  readonly statusTabs = ['All', 'Active', 'Completed'];

  // ── Pagination (server-side) ──────────────────────────────────────
  // Source of truth comes from the service's goalsPage / goalsTotalCount
  // signals, which the service fills via loadPagedGoals().
  page     = signal(1);
  pageSize = 4;

  pagedGoals = computed(() => this.goalService.goalsPage());
  totalCount = computed(() => this.goalService.goalsTotalCount());
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.goalService.loadPagedGoals(p, this.pageSize, this.filterStatus());
  }
  pageNumbers(): number[] {
    const t = this.totalPages(), c = this.page(), r: number[] = [];
    for (let i = Math.max(1, c - 2); i <= Math.min(t, c + 2); i++) r.push(i);
    return r;
  }

  ngOnInit(): void {
    // Load full list for dashboard counts (active/completed) AND a paged
    // view for the grid. Paged endpoint is what drives display.
    this.goalService.loadAll();
    this.goalService.loadPagedGoals(1, this.pageSize, this.filterStatus());
  }

  setFilter(status: string): void {
    this.filterStatus.set(status);
    this.page.set(1);
    this.goalService.loadPagedGoals(1, this.pageSize, status);
  }

  onDeleteRequest(id:string):void{
    this.pendingDeleteId = id;
    this.showConfirm = true;
  }
    onDeleteConfirmed(): void {
    this.goalService.remove(this.pendingDeleteId);
    this.showConfirm     = false;
    this.pendingDeleteId = '';
  }

  onDeleteCancelled(): void {
    this.showConfirm     = false;
    this.pendingDeleteId = '';
  }
}
