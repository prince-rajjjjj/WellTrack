import { Pipe, PipeTransform } from '@angular/core';

/**
 * Maps a status string to a Bootstrap text-color utility class.
 *
 * Usage in templates:
 *   <span [class]="goal.status | statusColor">{{ goal.status }}</span>
 */
@Pipe({
  name:       'statusColor',
  standalone: true,
  pure:       true,
})
export class StatusColorPipe implements PipeTransform {
  transform(status: string): string {
    const map: Record<string, string> = {
      active:     'text-success',
      completed:  'text-primary',
      pending:    'text-warning',
      inactive:   'text-secondary',
      cancelled:  'text-danger',
      enrolled:   'text-info',
      inprogress: 'text-warning',
      approved:   'text-success',
      rejected:   'text-danger',
    };
    return map[status?.toLowerCase().replace(/\s+/g, '')] ?? 'text-secondary';
  }
}
