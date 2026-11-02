import { Component, Input, Output, EventEmitter, OnInit, inject, signal, computed } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ProgramService } from '../../services/program';
import { AuthService } from '../../../../core/services/auth';
import { ProgramDto } from '../../models/program.model';
import { DatePicker } from '../../../../shared/components/date-picker/date-picker';

// "YYYY-MM-DD" string of today, in local time, useful for [min] and comparisons.
function todayIso(): string {
  const d = new Date();
  d.setHours(0, 0, 0, 0);
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  return `${y}-${m}-${dd}`;
}

function notInPast(control: AbstractControl): ValidationErrors | null {
  const v = control.value;
  if (!v) return null;
  return v < todayIso() ? { pastDate: true } : null;
}

function endAfterStart(group: AbstractControl): ValidationErrors | null {
  const start = group.get('startDate')?.value;
  const end   = group.get('endDate')?.value;
  if (!start || !end) return null;
  return end < start ? { endBeforeStart: true } : null;
}

@Component({
  selector: 'app-program-form-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, DatePicker],
  templateUrl: './program-form-modal.html',
})
export class ProgramFormModal implements OnInit {
  @Input() program?: ProgramDto;
  @Output() closed = new EventEmitter<boolean>();

  private fb             = inject(FormBuilder);
  private programService = inject(ProgramService);
  private auth           = inject(AuthService);

  loading = signal(false);
  error   = signal<string | null>(null);
  todayMin = todayIso();

  get isEdit() { return !!this.program; }

  // Past dates are blocked on CREATE only — when editing an in-flight program
  // we can't retroactively reject the historical startDate.
  form = this.fb.group({
    title:       ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', [Validators.required, Validators.maxLength(1000)]],
    startDate:   ['', [Validators.required]],
    endDate:     ['', [Validators.required]],
  }, { validators: endAfterStart });

  get title()       { return this.form.controls.title; }
  get description() { return this.form.controls.description; }
  get startDate()   { return this.form.controls.startDate; }
  get endDate()     { return this.form.controls.endDate; }

  ngOnInit(): void {
    if (!this.isEdit) {
      // Block past dates on create only.
      this.startDate.addValidators(notInPast);
      this.endDate.addValidators(notInPast);
      this.startDate.updateValueAndValidity({ emitEvent: false });
      this.endDate.updateValueAndValidity({ emitEvent: false });
    }
    if (this.program) {
      this.form.patchValue({
        title:       this.program.title,
        description: this.program.description,
        startDate:   this.program.startDate.substring(0, 10),
        endDate:     this.program.endDate.substring(0, 10),
      });
    }
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set(null);
    const raw = this.form.getRawValue();

    const call$ = this.isEdit
      ? this.programService.update(this.program!.id, {
          title: raw.title!, description: raw.description!,
          startDate: raw.startDate!, endDate: raw.endDate!,
        })
      : this.programService.create({
          title: raw.title!, description: raw.description!,
          hrID: this.auth.getCurrentUserId(),
          startDate: raw.startDate!, endDate: raw.endDate!,
        });

    call$.subscribe({
      next:  () => this.closed.emit(true),
      error: (err) => { this.error.set(err.message ?? 'Failed to save.'); this.loading.set(false); }
    });
  }

  cancel(): void { this.closed.emit(false); }
  onBackdropClick(e: MouseEvent): void { if (e.target === e.currentTarget) this.cancel(); }
}
