import { Component, Input } from '@angular/core';
import { CommonModule }     from '@angular/common';

@Component({
  selector:    'app-progress-bar',
  standalone:  true,
  imports:     [CommonModule],
  templateUrl: './progress-bar.html',
  styleUrl:    './progress-bar.scss',
})
export class ProgressBar {
  /** Progress value 0-100 */
  @Input() value  = 0;

  /** Label shown inside / next to the bar. Defaults to value% */
  @Input() label?: string;

  /** Bootstrap contextual color: 'success' | 'warning' | 'danger' | 'info' | 'primary' */
  @Input() color: 'success' | 'warning' | 'danger' | 'info' | 'primary' = 'primary';

  /** Show percentage text inside the bar */
  @Input() showText = true;

  /** Bar height in px */
  @Input() height = 10;

  get clampedValue(): number {
    return Math.min(100, Math.max(0, this.value));
  }

  get displayLabel(): string {
    return this.label ?? `${this.clampedValue}%`;
  }

  /** Auto-color by value when no explicit color is set by caller */
  get autoColor(): string {
    if (this.color !== 'primary') return this.color;
    if (this.clampedValue >= 80) return 'success';
    if (this.clampedValue >= 40) return 'warning';
    if (this.clampedValue >  0)  return 'info';
    return 'secondary';
  }
}
