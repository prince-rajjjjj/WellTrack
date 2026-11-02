import { Component, inject, signal } from '@angular/core';
import { FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../../core/services/auth';
import { LoginRequest } from '../../models/login.model';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [ReactiveFormsModule,RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {
  private fb     = inject(FormBuilder);
  private auth   = inject(AuthService);
  private router = inject(Router);
  

  isLoading = signal(false);
  error     = signal<string | null>(null);
  currentYear = new Date().getFullYear();
  showPassword = false;

  form = this.fb.group({
    email:    ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });
  
  get email()    { return this.form.controls.email; }
  get password() { return this.form.controls.password; }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.isLoading.set(true);
    this.error.set(null);

    const credentials = this.form.getRawValue() as LoginRequest;
    
    this.auth.login(credentials).subscribe({
      next: (res) => {
        if (res.succeeded) {
          this.router.navigate(['/dashboard']);
        } else {
          this.error.set(res.message ?? 'Login failed');
          this.isLoading.set(false);
        }
      },
      error: (err) => {
        this.error.set(err.error?.message ?? 'Invalid email or password');
        this.isLoading.set(false);
      }
    });
  }
}