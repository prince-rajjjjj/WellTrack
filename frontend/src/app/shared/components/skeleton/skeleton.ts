import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Tiny reusable skeleton shapes used while data is loading.
 * Wraps Bootstrap's `placeholder-glow` so callers don't repeat markup.
 *
 * <wt-skeleton variant="stat-card" [count]="4"></wt-skeleton>
 * <wt-skeleton variant="row" [count]="5"></wt-skeleton>
 * <wt-skeleton variant="text" widths="70 40 90"></wt-skeleton>
 */
@Component({
  selector:   'wt-skeleton',
  standalone: true,
  imports:    [CommonModule],
  template: `
    @switch (variant) {
      <!-- Stat-card grid: matches the 4-up dashboard tiles. -->
      @case ('stat-card') {
        <div class="row g-3">
          @for (i of repeat; track $index) {
            <div class="col-6 col-md-3">
              <div class="card border-0 shadow-sm rounded-4 p-3 placeholder-glow">
                <div class="d-flex align-items-center gap-3">
                  <span class="placeholder rounded-3" style="width:44px;height:44px;"></span>
                  <div class="flex-grow-1">
                    <span class="placeholder col-7 d-block mb-2"></span>
                    <span class="placeholder col-4 d-block" style="height:1.25rem;"></span>
                  </div>
                </div>
                <span class="placeholder col-8 d-block mt-3" style="height:0.45rem;"></span>
              </div>
            </div>
          }
        </div>
      }

      <!-- Single row block: for tables / lists. -->
      @case ('row') {
        <div class="d-flex flex-column gap-2 placeholder-glow">
          @for (i of repeat; track $index) {
            <span class="placeholder rounded-3 col-12" style="height:60px;"></span>
          }
        </div>
      }

      <!-- Card grid (programs / activities). -->
      @case ('card') {
        <div class="row g-3">
          @for (i of repeat; track $index) {
            <div class="col-md-6 col-lg-4">
              <div class="card border-0 shadow-sm rounded-4 p-4 placeholder-glow">
                <span class="placeholder col-7 mb-3 d-block" style="height:1.25rem;"></span>
                <span class="placeholder col-12 d-block mb-2"></span>
                <span class="placeholder col-10 d-block mb-2"></span>
                <span class="placeholder col-6 d-block mt-3" style="height:0.45rem;"></span>
              </div>
            </div>
          }
        </div>
      }

      <!-- Plain text lines, configurable widths via "widths" input (e.g. "70 40 90"). -->
      @case ('text') {
        <div class="placeholder-glow">
          @for (w of textWidths; track $index) {
            <span class="placeholder d-block mb-2" [style.width.%]="w"></span>
          }
        </div>
      }
    }
  `,
})
export class Skeleton {
  /** Pick the visual shape — controls which template branch renders. */
  @Input() variant: 'stat-card' | 'row' | 'card' | 'text' = 'row';
  /** How many skeleton items to render. */
  @Input() count = 1;
  /** For variant="text": space-separated widths (%) per line. e.g. "70 40 90". */
  @Input() widths = '80 60 40';

  /** Cheap iterator — `@for` only needs *something* iterable. */
  get repeat(): number[] {
    return Array.from({ length: this.count }, (_, i) => i);
  }

  get textWidths(): number[] {
    return this.widths.split(/\s+/).map(s => +s).filter(n => !isNaN(n));
  }
}
