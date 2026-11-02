import { Component, EventEmitter, Input, Output, inject, signal, OnInit } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { UserAdminService } from '../../services/user-admin';
import { UserProfile, UserRole, UserStatus, UpdateUserRequest } from '../../models/user.model';

@Component({
  selector: 'app-edit-user-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './edit-user-modal.html'
})
export class EditUserModalComponent implements OnInit {
  @Input({ required: true }) user!: UserProfile;
  @Output() closed = new EventEmitter<boolean>();

  private fb          = inject(FormBuilder);
  private userService = inject(UserAdminService);

  loading = signal(false);
  error   = signal<string | null>(null);

  roles = [
    { value: UserRole.Admin,             label: 'Admin' },
    { value: UserRole.HR,                label: 'HR Manager' },
    { value: UserRole.Coach,             label: 'Wellness Coach' },
    { value: UserRole.ComplianceOfficer, label: 'Compliance Officer' },
    { value: UserRole.Employee,          label: 'Employee' }
  ];

  statuses = [
    { value: UserStatus.Active,   label: 'Active' },
    { value: UserStatus.Inactive, label: 'Inactive' },
    { value: UserStatus.Pending,  label: 'Pending' },
    { value: UserStatus.Archived, label: 'Archived' }
  ];

  form = this.fb.group({
    name:   ['', [Validators.required, Validators.minLength(2)]],
    phone:  ['', [
      Validators.required,
      Validators.pattern(/^\d{10}$/)                  // exactly 10 digits
    ]],
    role:   [UserRole.Employee, [Validators.required]],
    status: [UserStatus.Active,  [Validators.required]]
  });

  get name()  { return this.form.controls.name; }
  get phone() { return this.form.controls.phone; }

  ngOnInit(): void {
    // Strip "+91" prefix from saved value so input shows just digits
    const phoneDigits = (this.user.phone ?? '').replace(/^\+91/, '');

    this.form.patchValue({
      name:   this.user.name,
      phone:  phoneDigits,
      role:   UserRole[this.user.role as keyof typeof UserRole],
      status: UserStatus[this.user.status as keyof typeof UserStatus]
    });
  }

  submit(): void {
    if (this.loading()) return;                       // guard double-click
    this.loading.set(true);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      this.loading.set(false);
      return;
    }

    this.error.set(null);

    const raw = this.form.getRawValue();
    const dto: UpdateUserRequest = {
      name:   raw.name!,
      phone:  `+91${raw.phone}`,                      // re-add prefix on save
      role:   Number(raw.role),
      status: Number(raw.status)
    };

    this.userService.updateUser(this.user.id, dto).subscribe({
      next: () => {
        this.userService.loadUsers().subscribe({
          next:  () => { this.loading.set(false); this.closed.emit(true); },
          error: () => { this.loading.set(false); this.closed.emit(true); }
        });
      },
      error: (err) => {
        this.error.set(err.error?.message ?? err.message ?? 'Failed to update user.');
        this.loading.set(false);
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