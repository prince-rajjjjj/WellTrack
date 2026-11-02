import { Injectable, signal, computed } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SpinnerService {
  /** Counter so nested show/hide calls work correctly */
  private _count = signal(0);

  /** True when at least one pending request is active */
  readonly isLoading = computed(() => this._count() > 0);

  show(): void {
    this._count.update(c => c + 1);
  }

  hide(): void {
    this._count.update(c => Math.max(0, c - 1));
  }
}
