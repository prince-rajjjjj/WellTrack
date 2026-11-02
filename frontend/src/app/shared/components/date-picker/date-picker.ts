import { Component, ElementRef, HostListener, Input, computed, forwardRef, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

interface DayCell {
  day: number;
  date: Date;
  inMonth: boolean;
  isToday: boolean;
  isSelected: boolean;
  disabled: boolean;
}

const MONTHS = ['January', 'February', 'March', 'April', 'May', 'June',
                'July', 'August', 'September', 'October', 'November', 'December'];
const WEEKDAYS = ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'];

function pad(n: number): string { return String(n).padStart(2, '0'); }
function startOfDay(d: Date): Date { const c = new Date(d); c.setHours(0, 0, 0, 0); return c; }
function sameDate(a: Date, b: Date): boolean {
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate();
}

@Component({
  selector: 'app-date-picker',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './date-picker.html',
  styleUrl: './date-picker.scss',
  providers: [{
    provide: NG_VALUE_ACCESSOR,
    useExisting: forwardRef(() => DatePicker),
    multi: true,
  }],
})
export class DatePicker implements ControlValueAccessor {
  @Input() mode: 'date' | 'datetime' = 'date';
  @Input() min: string | null = null;
  @Input() max: string | null = null;
  @Input() placeholder = 'Select a date';
  @Input() invalid = false;

  private elementRef = inject(ElementRef);
  private onChange: (v: string) => void = () => {};
  private onTouched: () => void = () => {};

  readonly months = MONTHS;
  readonly weekdays = WEEKDAYS;

  isOpen        = signal(false);
  disabled      = signal(false);
  rawValue      = signal<string>('');
  selectedDate  = signal<Date | null>(null);
  viewYear      = signal(new Date().getFullYear());
  viewMonth     = signal(new Date().getMonth());
  hour          = signal(9);
  minute        = signal(0);
  placement     = signal<'bottom' | 'top'>('bottom');

  // What to show in the fake input
  display = computed(() => {
    const d = this.selectedDate();
    if (!d) return '';
    const datePart = `${d.getDate()} ${MONTHS[d.getMonth()].slice(0, 3)} ${d.getFullYear()}`;
    if (this.mode === 'date') return datePart;
    return `${datePart} · ${pad(this.hour())}:${pad(this.minute())}`;
  });

  // 6×7 grid of day cells for the current view
  grid = computed<DayCell[]>(() => {
    const y = this.viewYear();
    const m = this.viewMonth();
    const today   = startOfDay(new Date());
    const minDate = this.min ? startOfDay(new Date(this.min)) : null;
    const maxDate = this.max ? startOfDay(new Date(this.max)) : null;
    const sel = this.selectedDate();

    const firstOfMonth = new Date(y, m, 1);
    const start = new Date(firstOfMonth);
    start.setDate(firstOfMonth.getDate() - firstOfMonth.getDay());

    const cells: DayCell[] = [];
    for (let i = 0; i < 42; i++) {
      const d = new Date(start);
      d.setDate(start.getDate() + i);
      cells.push({
        day: d.getDate(),
        date: d,
        inMonth: d.getMonth() === m,
        isToday: sameDate(d, today),
        isSelected: !!sel && sameDate(d, sel),
        disabled: (!!minDate && d < minDate) || (!!maxDate && d > maxDate),
      });
    }
    return cells;
  });

  // ─── ControlValueAccessor ──────────────────────────────────
  writeValue(value: string): void {
    this.rawValue.set(value || '');
    if (value) {
      const d = new Date(value);
      if (!isNaN(d.getTime())) {
        this.selectedDate.set(d);
        this.viewYear.set(d.getFullYear());
        this.viewMonth.set(d.getMonth());
        if (this.mode === 'datetime') {
          this.hour.set(d.getHours());
          this.minute.set(d.getMinutes());
        }
      }
    } else {
      this.selectedDate.set(null);
    }
  }
  registerOnChange(fn: (v: string) => void): void { this.onChange = fn; }
  registerOnTouched(fn: () => void): void { this.onTouched = fn; }
  setDisabledState(disabled: boolean): void { this.disabled.set(disabled); }

  // ─── Interaction ───────────────────────────────────────────
  toggleOpen(): void {
    if (this.disabled()) return;
    if (!this.isOpen()) {
      // Sync view to a sensible month:
      //   1. The selected date if one exists
      //   2. Otherwise [min] if it's in the future (program starting later)
      //   3. Otherwise [max] if it's already in the past (program already ended)
      //   4. Otherwise today
      let d = this.selectedDate();
      if (!d) {
        const today = new Date();
        const minDate = this.min ? new Date(this.min) : null;
        const maxDate = this.max ? new Date(this.max) : null;
        if (minDate && minDate > today) d = minDate;
        else if (maxDate && maxDate < today) d = maxDate;
        else d = today;
      }
      this.viewYear.set(d.getFullYear());
      this.viewMonth.set(d.getMonth());
      this.computePlacement();
      this.isOpen.set(true);
    } else {
      // Closing via the trigger — commit any picked date so the form sees it.
      if (this.selectedDate()) this.confirm();
      else { this.isOpen.set(false); this.onTouched(); }
    }
  }

  // Pop up vs down based on space available in viewport.
  private computePlacement(): void {
    const host = this.elementRef.nativeElement as HTMLElement;
    const trigger = host.querySelector('.dp-input') as HTMLElement | null;
    if (!trigger) { this.placement.set('bottom'); return; }
    const rect = trigger.getBoundingClientRect();
    const popoverHeight = this.mode === 'datetime' ? 480 : 420;
    const spaceBelow = window.innerHeight - rect.bottom;
    const spaceAbove = rect.top;
    this.placement.set(spaceBelow < popoverHeight && spaceAbove > spaceBelow ? 'top' : 'bottom');
  }

  prevMonth(): void {
    const m = this.viewMonth();
    if (m === 0) { this.viewMonth.set(11); this.viewYear.update(y => y - 1); }
    else this.viewMonth.set(m - 1);
  }
  nextMonth(): void {
    const m = this.viewMonth();
    if (m === 11) { this.viewMonth.set(0); this.viewYear.update(y => y + 1); }
    else this.viewMonth.set(m + 1);
  }

  pickDay(cell: DayCell): void {
    if (cell.disabled) return;
    this.selectedDate.set(cell.date);
    this.viewMonth.set(cell.date.getMonth());
    this.viewYear.set(cell.date.getFullYear());
    if (this.mode === 'date') this.confirm();
  }

  selectToday(): void {
    const t = new Date();
    if (this.min && startOfDay(t) < startOfDay(new Date(this.min))) return;
    if (this.max && startOfDay(t) > startOfDay(new Date(this.max))) return;
    this.selectedDate.set(t);
    this.viewMonth.set(t.getMonth());
    this.viewYear.set(t.getFullYear());
    if (this.mode === 'datetime') {
      this.hour.set(t.getHours());
      this.minute.set(t.getMinutes());
    }
  }

  clear(): void {
    this.selectedDate.set(null);
    this.rawValue.set('');
    this.onChange('');
    this.isOpen.set(false);
  }

  onHourChange(v: string): void { this.hour.set(this.clamp(parseInt(v, 10), 0, 23)); }
  onMinuteChange(v: string): void { this.minute.set(this.clamp(parseInt(v, 10), 0, 59)); }
  private clamp(n: number, lo: number, hi: number): number {
    if (Number.isNaN(n)) return 0;
    return Math.min(hi, Math.max(lo, n));
  }

  confirm(): void {
    const d = this.selectedDate();
    if (!d) { this.isOpen.set(false); return; }
    const out = this.mode === 'date'
      ? `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`
      : `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(this.hour())}:${pad(this.minute())}`;
    this.rawValue.set(out);
    // Mark touched first so validators run with the new value already present,
    // then push the value through the CVA. Order matters: some Angular versions
    // flag the field invalid if onChange runs before the FormControl is observed.
    this.onTouched();
    this.onChange(out);
    this.isOpen.set(false);
  }

  // ─── Outside click closes the popover ──────────────────────
  // If there's a picked date in the popover when it closes via outside-click,
  // commit it (otherwise the user sees the date in the trigger but the form
  // stays empty — a confusing "Date is required" state).
  @HostListener('document:click', ['$event'])
  onDocumentClick(ev: MouseEvent): void {
    if (!this.elementRef.nativeElement.contains(ev.target)) {
      if (this.isOpen()) {
        if (this.selectedDate()) {
          this.confirm();
        } else {
          this.isOpen.set(false);
          this.onTouched();
        }
      }
    }
  }
}
