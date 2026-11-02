import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter, withPreloading, PreloadAllModules }     from '@angular/router';
import { provideHttpClient, withInterceptors }                  from '@angular/common/http';
import { provideAnimations }                                     from '@angular/platform-browser/animations';
import { provideToastr }                                         from 'ngx-toastr';

import { routes }          from './app.routes';
import { authInterceptor } from './core/interceptors/auth-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),

    // ─── Router ───────────────────────────────────────────────────
    // PreloadAllModules downloads every lazy chunk in the background
    // right after the initial render, so the first click on any nav
    // item is instant (no bundle fetch latency).
    provideRouter(routes, withPreloading(PreloadAllModules)),

    // ─── HTTP client + auth interceptor ───────────────────────────
    // authInterceptor attaches Bearer token to every request automatically
    provideHttpClient(withInterceptors([authInterceptor])),

    // ─── Animations (required by ngx-toastr) ──────────────────────
    provideAnimations(),

    // ─── Toast notifications ──────────────────────────────────────
    provideToastr({
      timeOut:            3000,
      positionClass:      'toast-top-right',
      preventDuplicates:  true,
      progressBar:        true,
      closeButton:        true,
    }),
  ],
};
