import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule, AbstractControl } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { toSignal } from '@angular/core/rxjs-interop';
import { UserService } from '../../services/user';
import { UserProfile } from '../../models/user.model';
import { AuthService } from '../../../../core/services/auth';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  private fb          = inject(FormBuilder);
  private userService = inject(UserService);
  private auth        = inject(AuthService);

  profile = signal<UserProfile | null>(null);
  activeTab = signal<'info' | 'password'>('info');

  // ─── Profile form ──────────────────────────────────────────
  profileLoading = signal(false);
  profileError   = signal<string | null>(null);
  profileSuccess = signal<string | null>(null);

  profileForm = this.fb.group({
    name:  ['', [Validators.required, Validators.minLength(2)]],
    phone: ['', [
      Validators.required,
      Validators.pattern(/^\d{10}$/)
    ]]
  });

  // ─── Password form ─────────────────────────────────────────
  passwordLoading = signal(false);
  passwordError   = signal<string | null>(null);
  passwordSuccess = signal<string | null>(null);
  showCurrent = false;
  showNew     = false;
  showConfirm = false;

  passwordForm = this.fb.group({
    currentPassword: ['', [Validators.required]],
    newPassword:     ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*]).+$/)
    ]],
    confirmPassword: ['', [Validators.required]]
  }, { validators: [this.passwordsMatch, this.passwordsDifferent] });

  get name()            { return this.profileForm.controls.name; }
  get phone()           { return this.profileForm.controls.phone; }
  get currentPassword() { return this.passwordForm.controls.currentPassword; }
  get newPassword()     { return this.passwordForm.controls.newPassword; }
  get confirmPassword() { return this.passwordForm.controls.confirmPassword; }

  private newPasswordValue = toSignal(this.newPassword.valueChanges, { initialValue: '' });

  hasMinLength = computed(() => (this.newPasswordValue()?.length ?? 0) >= 8);
  hasLowercase = computed(() => /[a-z]/.test(this.newPasswordValue() ?? ''));
  hasUppercase = computed(() => /[A-Z]/.test(this.newPasswordValue() ?? ''));
  hasNumber    = computed(() => /\d/.test(this.newPasswordValue() ?? ''));
  hasSpecial   = computed(() => /[!@#$%^&*]/.test(this.newPasswordValue() ?? ''));

  passwordStrength = computed(() => {
    const checks = [
      this.hasMinLength(), this.hasLowercase(), this.hasUppercase(),
      this.hasNumber(), this.hasSpecial()
    ];
    return checks.filter(Boolean).length;
  });

  strengthLabel = computed(() => {
    const s = this.passwordStrength();
    if (s <= 1) return { label: 'Very weak', color: 'danger' };
    if (s === 2) return { label: 'Weak',     color: 'danger' };
    if (s === 3) return { label: 'Fair',     color: 'warning' };
    if (s === 4) return { label: 'Strong',   color: 'success' };
    return       { label: 'Very strong', color: 'success' };
  });

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.userService.getMyProfile().subscribe({
      next: (res) => {
        if (res.succeeded && res.data) {
          this.profile.set(res.data);
          this.auth.setName(res.data.name);

          const phoneDigits = (res.data.phone ?? '').replace(/^\+91/, '');
          this.profileForm.patchValue({
            name:  res.data.name,
            phone: phoneDigits
          });
        }
      },
      error: () => this.profileError.set('Failed to load profile')
    });
  }

  /** Confirm password must equal new password. */
  passwordsMatch(group: AbstractControl) {
    const n = group.get('newPassword')?.value;
    const c = group.get('confirmPassword')?.value;
    return n === c ? null : { mismatch: true };
  }

  /** New password must NOT equal the current password. */
  passwordsDifferent(group: AbstractControl) {
    const current = group.get('currentPassword')?.value;
    const newPwd  = group.get('newPassword')?.value;
    if (!current || !newPwd) return null;
    return current === newPwd ? { sameAsCurrent: true } : null;
  }

  // ─── Save profile (name + phone) ───────────────────────────
  saveProfile(): void {
    if (this.profileLoading()) return;
    this.profileLoading.set(true);

    if (this.profileForm.invalid) {
      this.profileForm.markAllAsTouched();
      this.profileLoading.set(false);
      return;
    }

    this.profileError.set(null);
    this.profileSuccess.set(null);

    const raw = this.profileForm.getRawValue();
    const dto = {
      name:  raw.name!,
      phone: `+91${raw.phone}`
    };

    this.userService.updateProfile(dto).subscribe({
      next: (res) => {
        if (res.succeeded && res.data) {
          this.profile.set(res.data);
          this.auth.setName(res.data.name);
          this.profileSuccess.set('Profile updated successfully.');
        } else {
          this.profileError.set(res.message ?? 'Update failed');
        }
        this.profileLoading.set(false);
      },
      error: (err) => {
        this.profileError.set(err.error?.message ?? 'Update failed');
        this.profileLoading.set(false);
      }
    });
  }

  // ─── Save new password ────────────────────────────────────
  savePassword(): void {
    if (this.passwordLoading()) return;
    this.passwordLoading.set(true);

    if (this.passwordForm.invalid) {
      this.passwordForm.markAllAsTouched();
      this.passwordLoading.set(false);
      return;
    }

    this.passwordError.set(null);
    this.passwordSuccess.set(null);

    const { currentPassword, newPassword } = this.passwordForm.getRawValue();

    this.userService.changePassword({
      currentPassword: currentPassword!,
      newPassword:     newPassword!
    }).subscribe({
      next: () => {
        this.passwordSuccess.set('Password changed successfully.');
        this.passwordForm.reset();
        this.passwordLoading.set(false);
      },
      error: (err) => {
        this.passwordError.set(err.error?.message ?? 'Current password is incorrect.');
        this.passwordLoading.set(false);
      }
    });
  }

  initial(): string {
    return this.profile()?.name?.charAt(0).toUpperCase() ?? 'U';
  }
}