import { Component, inject } from '@angular/core';
import { Router }             from '@angular/router';
import { CommonModule }       from '@angular/common';
import {
  ReactiveFormsModule,
  FormBuilder,
  Validators,
  AbstractControl,
  ValidatorFn,
} from '@angular/forms';
import { GoalService }    from '../../services/goal';
import { CreateGoalDto }  from '../../models/goal.model';

@Component({
  selector:    'app-goal-form',
  standalone:  true,
  imports:     [CommonModule, ReactiveFormsModule],
  templateUrl: './goal-form.html',
  styleUrl:    './goal-form.scss',
})
export class GoalForm {

  private fb           = inject(FormBuilder);
  private router       = inject(Router);
  readonly goalService = inject(GoalService);

  // ── DATE HELPERS ──────────────────────────────────────────────────
  /** Today as YYYY-MM-DD — used as [min] on the start date input */
  readonly todayStr = new Date().toISOString().split('T')[0];

  /** One day after whatever is selected in startDate — used as [min] on end date */
  get minEndDateStr(): string {
    const start = this.f['startDate'].value;
    if (!start) return this.todayStr;
    const [y, m, d] = start.split('-').map(Number);
    const next = new Date(y, m - 1, d + 1);
    return [
      next.getFullYear(),
      String(next.getMonth() + 1).padStart(2, '0'),
      String(next.getDate()).padStart(2, '0'),
    ].join('-');
  }

  // ── CUSTOM VALIDATORS ─────────────────────────────────────────────
  /** Rejects any date string earlier than today */
  private noBackDate: ValidatorFn = (ctrl: AbstractControl) => {
    if (!ctrl.value) return null;
    return ctrl.value >= this.todayStr ? null : { pastDate: true };
  };

  /** Form-group validator: end date must be strictly after start date */
  private endAfterStart: ValidatorFn = (group: AbstractControl) => {
    const start = group.get('startDate')?.value;
    const end   = group.get('endDate')?.value;
    if (!start || !end) return null;
    return end > start ? null : { endBeforeStart: true };
  };

  // ── FORM ──────────────────────────────────────────────────────────
  form = this.fb.group({
    description: ['',                    [Validators.required, Validators.minLength(5)]],
    targetValue: [null as number | null, [Validators.required, Validators.min(1)]],
    startDate:   ['',                    [Validators.required, this.noBackDate]],
    endDate:     ['',                    [Validators.required]],
  }, { validators: this.endAfterStart });

  // ── HELPERS ───────────────────────────────────────────────────────
  get f() { return this.form.controls; }

  isInvalid(field: string): boolean {
    const control = this.form.get(field);
    return !!(control?.invalid && control?.touched);
  }

  // ── SUBMIT ────────────────────────────────────────────────────────
  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const dto: CreateGoalDto = {
      employeeId:  '',  // service overwrites from JWT
      description: this.f['description'].value!,
      targetValue: this.f['targetValue'].value!,
      startDate:   this.f['startDate'].value!,
      endDate:     this.f['endDate'].value!,
    };

    this.goalService.create(dto, () => {
      this.router.navigate(['/participation/goals']);
    });
  }

  // ── CANCEL ────────────────────────────────────────────────────────
  onCancel(): void {
    this.router.navigate(['/participation/goals']);
  }
}
