import { Injectable, inject } from '@angular/core';
import { ToastrService }      from 'ngx-toastr';

/**
 * Thin wrapper around ngx-toastr so feature components only depend on ToastService,
 * not directly on ToastrService. Swap the underlying library in one place if needed.
 */
@Injectable({ providedIn: 'root' })
export class ToastService {
  private toastr = inject(ToastrService);

  success(message: string, title = 'Success'): void {
    this.toastr.success(message, title);
  }

  error(message: string, title = 'Error'): void {
    this.toastr.error(message, title);
  }

  info(message: string, title = 'Info'): void {
    this.toastr.info(message, title);
  }

  warning(message: string, title = 'Warning'): void {
    this.toastr.warning(message, title);
  }

  /** Show error messages from ApiResponse.errors map */
  apiErrors(errors: { [field: string]: string[] } | null | undefined): void {
    if (!errors) return;
    Object.values(errors)
      .flat()
      .forEach(msg => this.toastr.error(msg, 'Validation Error'));
  }
}
