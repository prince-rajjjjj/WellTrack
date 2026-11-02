import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule, DatePipe }  from '@angular/common';
import { FormsModule }             from '@angular/forms';
import { Goal }                    from '../../models/goal.model';
import { GoalService }             from '../../services/goal';
import { ProgressBar }             from '../../../../shared/components/progress-bar/progress-bar';
import { StatusBadge }             from '../../../../shared/components/status-badge/status-badge';

@Component({
  selector:    'app-goal-card',
  standalone:  true,
  imports:     [CommonModule, DatePipe, ProgressBar, StatusBadge, FormsModule],
  templateUrl: './goal-card.html',
  styleUrl:    './goal-card.scss',
})
export class GoalCard {

  @Input({ required: true }) goal!: Goal;
  @Output() deleted = new EventEmitter<string>();

  private goalService = inject(GoalService);

  protected Math = Math;
  showProgressInput = false;
  showExtendInput   = false;
  progressError     = '';
  newEndDate        = '';

  get todayStr(): string    { return new Date().toISOString().split('T')[0]; }
  get minEndDate(): string  { return new Date(this.goal.endDate).toISOString().split('T')[0]; }

  get isActive(): boolean { return this.goal.status === 'Active'; }

  get daysLeft(): number {
    const diff = new Date(this.goal.endDate).getTime() - Date.now();
    return Math.max(0, Math.ceil(diff / 86400000));
  }

  onDeleteClick(): void {
    this.deleted.emit(this.goal.id);
  }

  saveProgress(value: string): void {
    const num = +value;
    if (!num || num <= 0) return;
    if (num > this.goal.targetValue) {
      this.progressError = `Cannot exceed target of ${this.goal.targetValue}`;
      return;
    }
    this.progressError = '';
    this.goalService.updateProgress(this.goal.id, { currentValue: num });
    this.showProgressInput = false;
  }

  saveEndDate(): void {
    if (!this.newEndDate) return;
    this.goalService.extendDeadline(this.goal.id, this.newEndDate);
    this.showExtendInput = false;
    this.newEndDate = '';
  }
}
