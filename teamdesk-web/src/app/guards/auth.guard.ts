import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  return auth.isLoggedIn()
    ? true                                 // connecté → on affiche la page
    : router.createUrlTree(['/login']);    // sinon → redirection vers /login
};
export const managerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isLoggedIn()) return router.createUrlTree(['/login']);    // pas connecté → login
  return auth.isManager() ? true : router.createUrlTree(['/requests']); // pas Manager → liste
};