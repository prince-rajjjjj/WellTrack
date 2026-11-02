import { Component, Input, Output, EventEmitter, OnInit, inject, signal, computed, DestroyRef } from '@angular/core';
import { AbstractControl, FormBuilder, ValidationErrors, Validators, ReactiveFormsModule } from '@angular/forms';

// Local-time "YYYY-MM-DDTHH:mm" string of "now" for datetime-local [min].
function nowIsoMinute(): string {
  const d = new Date();
  d.setSeconds(0, 0);
  const y = d.getFullYear();
  const m = String(d.getMonth() + 1).padStart(2, '0');
  const dd = String(d.getDate()).padStart(2, '0');
  const hh = String(d.getHours()).padStart(2, '0');
  const mm = String(d.getMinutes()).padStart(2, '0');
  return `${y}-${m}-${dd}T${hh}:${mm}`;
}

function notInPastDateTime(control: AbstractControl): ValidationErrors | null {
  const v = control.value;
  if (!v) return null;
  return new Date(v).getTime() < Date.now() ? { pastDate: true } : null;
}

// Format a Date as "YYYY-MM-DDTHH:mm" in local time — what datetime-local
// inputs (and our DatePicker) expect for [min]/[max].
function toLocalIsoMinute(d: Date): string {
  const pad = (n: number) => String(n).padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}` +
         `T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}

// Validator: must be a recognised YouTube URL (watch / youtu.be / embed forms).
// Returns null when the field is empty (so "required" handles that separately).
function youtubeUrl(control: AbstractControl): ValidationErrors | null {
  const v = (control.value ?? '').toString().trim();
  if (!v) return null;
  // Allow https://(www.)?youtube.com/watch?v=ID,  /embed/ID,  /shorts/ID,  https://youtu.be/ID
  const re = /^https?:\/\/(www\.)?(youtube\.com\/(watch\?v=|embed\/|shorts\/)[A-Za-z0-9_-]{6,}|youtu\.be\/[A-Za-z0-9_-]{6,})/i;
  return re.test(v) ? null : { youtubeUrl: true };
}

// Validator: activity datetime must be inside the program's [startDate, endDate] window.
function withinProgramRange(startIso: string, endIso: string) {
  return (control: AbstractControl): ValidationErrors | null => {
    if (!control.value) return null;
    const t = new Date(control.value).getTime();
    const s = new Date(startIso).getTime();
    // End date is inclusive — push to end-of-day so 11:59 PM is still valid
    const endDay = new Date(endIso);
    endDay.setHours(23, 59, 59, 999);
    const e = endDay.getTime();
    return t < s || t > e ? { outOfProgramRange: true } : null;
  };
}
import { CommonModule } from '@angular/common';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { ActivityService } from '../../services/activity';
import { ProgramService } from '../../services/program';
import { UserAdminService } from '../../../identity/services/user-admin';
import { UserProfile } from '../../../identity/models/user.model';
import { ActivityDto, ActivityMode } from '../../models/activity.model';
import { DatePicker } from '../../../../shared/components/date-picker/date-picker';

export type ActivityModalMode = 'create' | 'edit' | 'assign-coach';

@Component({
  selector: 'app-activity-form-modal',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule, DatePicker],
  templateUrl: './activity-form-modal.html',
})
export class ActivityFormModal implements OnInit {
  @Input() mode: ActivityModalMode = 'create';
  @Input() activity?: ActivityDto;
  @Output() closed = new EventEmitter<boolean>();

  private fb               = inject(FormBuilder);
  private activityService  = inject(ActivityService);
  protected programService = inject(ProgramService);
  private userAdminService = inject(UserAdminService);
  private destroyRef       = inject(DestroyRef);

  loading        = signal(false);
  error          = signal<string | null>(null);
  loadingCoaches = signal(false);
  coaches        = signal<UserProfile[]>([]);
  nowMin         = nowIsoMinute();

  // The currently picked program (so we can clamp the activity's date to its window)
  selectedProgramId = signal<string>('');
  selectedProgram = computed(() => {
    const id = this.selectedProgramId();
    return this.programService.programs().find(p => p.id === id) ?? null;
  });

  // Writable — onProgramSelected() pushes new bounds whenever the user changes program.
  dateMin = signal<string | null>(null);
  dateMax = signal<string | null>(null);

  activityTypes = ['Yoga', 'Running', 'Workshop', 'Meditation', 'HIIT', 'Cycling', 'Swimming', 'Nutrition', 'Challenge', 'Other'];

  // Cascading location pickers — backend Location stays a string, joined with " > "
  cities    = ['Chennai'];
  buildings: Record<string, string[]> = {
    Chennai: ['SDB1', 'SDB2'],
  };
  floors    = ['Ground floor', '1st floor', '2nd floor', '3rd floor', '4th floor', '5th floor', '6th floor'];
  rooms     = ['1A', '2A', '3A', '4A', '5A', '6A', 'ODC 1', 'ODC 2', 'ODC 3', 'ODC 4', 'ODC 5', 'Auditorium', 'Conference Room A', 'Conference Room B'];

  buildingsForSelectedCity = (): string[] => {
    const c = this.createForm.controls.locCity.value || '';
    return this.buildings[c] ?? [];
  };

  createForm = this.fb.group({
    programID:         ['', Validators.required],
    title:             ['', [Validators.required, Validators.maxLength(200)]],
    description:       ['', [Validators.required, Validators.maxLength(1000)]],
    type:              ['', Validators.required],
    customType:        [''],
    days:              [7, [Validators.min(1)]],   // used only for Challenge-type activities
    date:              ['', [Validators.required, notInPastDateTime]],
    duration:          [60, [Validators.required, Validators.min(1)]],
    maxParticipants:   [30, [Validators.required, Validators.min(1)]],
    mode:              ['Offline' as ActivityMode, Validators.required],
    // Cascading location selection (combined into the `location` string on submit)
    locCity:           [''],
    locBuilding:       [''],
    locFloor:          [''],
    locRoom:           [''],
    location:          [''],
    videoUrl:          ['', youtubeUrl],
    assignedCoachID:   [''],
    assignedCoachName: [''],
  });

  assignForm = this.fb.group({
    coachID:   ['', Validators.required],
    coachName: ['', Validators.required],
  });

  ngOnInit(): void {
    if (this.programService.programs().length === 0) {
      this.programService.loadPrograms().pipe(takeUntilDestroyed(this.destroyRef)).subscribe();
    }
    this.loadCoaches();

    // Track the picked program so the date picker can clamp to its window.
    this.createForm.controls.programID.valueChanges
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(id => {
        this.selectedProgramId.set(id ?? '');
        this.onProgramSelected(id ?? '');
      });

    // In edit mode allow keeping the existing date (it might already be in the past).
    if (this.mode === 'edit') {
      this.createForm.controls.date.removeValidators(notInPastDateTime);
      this.createForm.controls.date.updateValueAndValidity({ emitEvent: false });
    }

    // Pre-fill the form when editing an existing activity.
    if (this.mode === 'edit' && this.activity) {
      const a = this.activity;
      const localIso = a.date ? new Date(a.date).toISOString().slice(0, 16) : '';
      const [city, building, floor, room] = (a.location ?? '').split(' > ');

      // If the saved type isn't one of our preset options, show it under "Other" with custom text.
      const isPreset = this.activityTypes.includes(a.type);
      const typeForDropdown = isPreset ? a.type : 'Other';
      const customTypeText  = isPreset ? '' : a.type;

      // If this turns out to be a challenge, derive days from the stored minutes.
      const looksLikeChallenge = typeForDropdown === 'Challenge'
        || (typeForDropdown === 'Other' && customTypeText.toLowerCase().includes('challenge'));
      const derivedDays = looksLikeChallenge ? Math.max(1, Math.round(a.duration / (24 * 60))) : 7;

      this.selectedProgramId.set(a.programID ?? '');
      this.createForm.patchValue({
        programID:         a.programID,
        title:             a.title,
        description:       a.description,
        type:              typeForDropdown,
        customType:        customTypeText,
        days:              derivedDays,
        date:              localIso,
        duration:          a.duration,
        maxParticipants:   a.maxParticipants,
        mode:              (a.mode as ActivityMode) ?? 'Offline',
        locCity:           city     ?? '',
        locBuilding:       building ?? '',
        locFloor:          floor    ?? '',
        locRoom:           room     ?? '',
        location:          a.location ?? '',
        videoUrl:          a.videoUrl ?? '',
        assignedCoachID:   a.assignedCoachID ?? '',
        assignedCoachName: a.assignedCoachName ?? '',
      });
    }
  }

  private onProgramSelected(programId: string): void {
    const dateCtrl = this.createForm.controls.date;

    // Base validators (always present), plus program-range when a program is picked.
    const base = this.mode === 'edit'
      ? [Validators.required]
      : [Validators.required, notInPastDateTime];

    if (!programId) {
      this.dateMin.set(this.nowMin);
      this.dateMax.set(null);
      dateCtrl.setValidators(base);
      dateCtrl.updateValueAndValidity({ emitEvent: false });
      return;
    }

    const program = this.programService.programs().find(p => p.id === programId);
    if (!program) return;

    // min = max(now, programStart) — can't schedule before the program window opens
    const programStart = new Date(program.startDate);
    const now          = new Date();
    const minDate      = programStart > now ? programStart : now;
    this.dateMin.set(toLocalIsoMinute(minDate));

    // max = program end date end-of-day — can't schedule past program window
    const endDate = new Date(program.endDate);
    endDate.setHours(23, 59, 0, 0);
    this.dateMax.set(toLocalIsoMinute(endDate));

    // Replace validators atomically: base + the range guard.
    dateCtrl.setValidators([...base, withinProgramRange(program.startDate, program.endDate)]);

    // If a date was already typed/picked and it's now outside the new range, clear it.
    const current = dateCtrl.value;
    if (current) {
      const d = new Date(current).getTime();
      if (d < programStart.getTime() || d > endDate.getTime()) {
        dateCtrl.reset('');
      }
    }

    dateCtrl.updateValueAndValidity({ emitEvent: false });
  }

  private loadCoaches(): void {
    // Reuse cached users only if any coaches are present (Admin already loaded the
    // full list). HR cannot hit /api/v1/users (Admin-only) so we fall back to the
    // dedicated /api/v1/users/coaches endpoint that's open to HR + Admin.
    const cached = this.userAdminService.users().filter(u => u.role === 'Coach' && u.status === 'Active');
    if (cached.length > 0) { this.coaches.set(cached); return; }

    this.loadingCoaches.set(true);
    this.userAdminService.loadActiveCoaches()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (coaches) => {
          this.coaches.set(coaches);
          this.loadingCoaches.set(false);
        },
        error: () => this.loadingCoaches.set(false),
      });
  }

  onModeChange(): void {
    const m = this.createForm.controls.mode.value;
    if (m === 'Offline') this.createForm.patchValue({ videoUrl: '' });
    if (m === 'Online')  this.createForm.patchValue({ location: '', locCity: '', locBuilding: '', locFloor: '', locRoom: '' });
  }

  onTypeChange(): void {
    if (this.createForm.controls.type.value !== 'Other') {
      this.createForm.patchValue({ customType: '' });
    }
  }

  // True when the activity type is "Challenge" (preset) or "Other" with a custom text containing "challenge".
  isChallenge(): boolean {
    const t = this.createForm.controls.type.value;
    if (t === 'Challenge') return true;
    if (t === 'Other') {
      const ct = this.createForm.controls.customType.value ?? '';
      return ct.toLowerCase().includes('challenge');
    }
    return false;
  }

  // City changes ⇒ wipe building (rooms/floors are static so leave alone)
  onLocCityChange(): void {
    this.createForm.patchValue({ locBuilding: '' });
    this.recomputeLocation();
  }

  recomputeLocation(): void {
    const f = this.createForm.controls;
    const parts = [f.locCity.value, f.locBuilding.value, f.locFloor.value, f.locRoom.value].filter(Boolean);
    this.createForm.patchValue({ location: parts.join(' > ') });
  }

  onCreateCoachSelected(coachId: string): void {
    if (!coachId) {
      this.createForm.patchValue({ assignedCoachID: '', assignedCoachName: '' });
      return;
    }
    const coach = this.coaches().find(c => c.id === coachId);
    if (coach) this.createForm.patchValue({ assignedCoachID: coach.id, assignedCoachName: coach.name });
  }

  onAssignCoachSelected(coachId: string): void {
    this.assignForm.controls.coachID.markAsTouched();
    if (!coachId) {
      this.assignForm.patchValue({ coachID: '', coachName: '' });
      return;
    }
    const coach = this.coaches().find(c => c.id === coachId);
    if (coach) this.assignForm.patchValue({ coachID: coach.id, coachName: coach.name });
  }

  submit(): void {
    if (this.mode === 'create' || this.mode === 'edit') this.submitCreateOrEdit();
    else this.submitAssignCoach();
  }

  private submitCreateOrEdit(): void {
    const m = this.createForm.controls.mode.value as ActivityMode;
    if (m === 'Offline' && !this.createForm.controls.location.value)  { this.error.set('Location is required for an offline activity.'); return; }
    if (m === 'Online'  && !this.createForm.controls.videoUrl.value)  { this.error.set('Video URL is required for an online activity.');  return; }
    if (m === 'Online'  && this.createForm.controls.videoUrl.hasError('youtubeUrl')) {
      this.error.set('Please enter a valid YouTube URL (e.g. https://www.youtube.com/watch?v=…).');
      return;
    }

    // "Other" requires a custom description
    if (this.createForm.controls.type.value === 'Other' && !this.createForm.controls.customType.value?.trim()) {
      this.error.set('Please describe what kind of activity this is.');
      return;
    }

    if (this.createForm.invalid) { this.createForm.markAllAsTouched(); return; }
    this.loading.set(true);
    this.error.set(null);
    const raw = this.createForm.getRawValue();

    // If "Other" selected, send the typed text as the actual type.
    const finalType = raw.type === 'Other' ? raw.customType!.trim() : raw.type!;

    // For challenges we collect "days" instead of minutes; convert before sending.
    const finalDuration = this.isChallenge()
      ? Math.max(1, raw.days ?? 1) * 24 * 60
      : raw.duration!;

    const dto = {
      programID:         raw.programID!,
      title:             raw.title!,
      description:       raw.description!,
      type:              finalType,
      date:              raw.date!,
      duration:          finalDuration,
      maxParticipants:   raw.maxParticipants!,
      mode:              m,
      location:          m === 'Offline' ? raw.location! : undefined,
      videoUrl:          m === 'Online'  ? raw.videoUrl! : undefined,
      assignedCoachID:   raw.assignedCoachID  || undefined,
      assignedCoachName: raw.assignedCoachName || undefined,
    };

    const op$ = this.mode === 'edit' && this.activity
      ? this.activityService.update(this.activity.id, dto)
      : this.activityService.create(dto);

    op$.subscribe({
      next:  () => this.closed.emit(true),
      error: (err) => { this.error.set(err.message ?? `Failed to ${this.mode} activity.`); this.loading.set(false); }
    });
  }

  private submitAssignCoach(): void {
    if (this.assignForm.invalid) { this.assignForm.markAllAsTouched(); return; }
    if (!this.activity) return;
    this.loading.set(true);
    this.error.set(null);
    const raw = this.assignForm.getRawValue();

    this.activityService.assignCoach(this.activity.id, {
      coachID:   raw.coachID!,
      coachName: raw.coachName!,
    }).subscribe({
      next:  () => this.closed.emit(true),
      error: (err) => { this.error.set(err.message ?? 'Failed to assign coach.'); this.loading.set(false); }
    });
  }

  cancel(): void { this.closed.emit(false); }
  onBackdropClick(e: MouseEvent): void { if (e.target === e.currentTarget) this.cancel(); }
}