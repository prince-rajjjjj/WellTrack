import { Component, EventEmitter, Output, inject, signal, computed } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { switchMap } from 'rxjs/operators';
import { UserAdminService } from '../../services/user-admin';
import { UserRole, CreateUserRequest } from '../../models/user.model';

@Component({
  selector: 'app-create-user-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './create-user-modal.html'
})
export class CreateUserModalComponent {
  @Output() closed = new EventEmitter<boolean>();

  private fb          = inject(FormBuilder);
  private userService = inject(UserAdminService);

  loading = signal(false);
  error   = signal<string | null>(null);
  showPassword = false;
  private isSubmitting = false;

  roles = [
    { value: UserRole.Admin,             label: 'Admin' },
    { value: UserRole.HR,                label: 'HR Manager' },
    { value: UserRole.Coach,             label: 'Wellness Coach' },
    { value: UserRole.ComplianceOfficer, label: 'Compliance Officer' },
    { value: UserRole.Employee,          label: 'Employee' }
  ];

  form = this.fb.group({
    name:     ['', [Validators.required, Validators.minLength(2)]],
    email:    ['', [Validators.required, Validators.email]],
    phone:    ['', [
      Validators.required,
      Validators.pattern(/^\d{10}$/)               // exactly 10 digits
    ]],
    role:     [UserRole.Employee, [Validators.required]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).+$/)
    ]]
  });

  get name()     { return this.form.controls.name; }
  get email()    { return this.form.controls.email; }
  get phone()    { return this.form.controls.phone; }
  get password() { return this.form.controls.password; }

  // Convert FormControl's valueChanges to a signal for reactive checks
  private passwordValue = toSignal(this.password.valueChanges, { initialValue: '' });

  hasMinLength = computed(() => (this.passwordValue()?.length ?? 0) >= 8);
  hasLowercase = computed(() => /[a-z]/.test(this.passwordValue() ?? ''));
  hasUppercase = computed(() => /[A-Z]/.test(this.passwordValue() ?? ''));
  hasNumber    = computed(() => /\d/.test(this.passwordValue() ?? ''));
  hasSpecial   = computed(() => /[!@#$%^&*]/.test(this.passwordValue() ?? ''));

  passwordStrength = computed(() => {
    const checks = [
      this.hasMinLength(),
      this.hasLowercase(),
      this.hasUppercase(),
      this.hasNumber(),
      this.hasSpecial()
    ];
    return checks.filter(Boolean).length;  // 0–5
  });

  strengthLabel = computed(() => {
    const s = this.passwordStrength();
    if (s <= 1) return { label: 'Very weak',   color: 'danger'  };
    if (s === 2) return { label: 'Weak',        color: 'danger'  };
    if (s === 3) return { label: 'Fair',        color: 'warning' };
    if (s === 4) return { label: 'Strong',      color: 'success' };
    return       { label: 'Very strong', color: 'success' };
  });

  submit(): void {
    if (this.isSubmitting) return;
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;
    this.loading.set(true);
    this.error.set(null);

    const raw = this.form.getRawValue();
    const dto: CreateUserRequest = {
      name:     raw.name!,
      email:    raw.email!,
      phone:    `+91${raw.phone}`,
      role:     Number(raw.role),
      password: raw.password!
    };

    this.userService
      .createUser(dto)
      .pipe(switchMap(() => this.userService.loadUsers()))
      .subscribe({
        next: () => {
          this.isSubmitting = false;
          this.closed.emit(true);
        },
        error: (err) => {
          this.error.set(err.message ?? 'Failed to create user.');
          this.loading.set(false);
          this.isSubmitting = false;
        }
      });
  }

  cancel(): void {
    this.closed.emit(false);
  }

  onBackdropClick(event: MouseEvent): void {
    if (event.target === event.currentTarget) this.cancel();
  }
}