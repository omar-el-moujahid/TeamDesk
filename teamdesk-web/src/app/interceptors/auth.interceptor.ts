import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const token = auth.getToken();

  // On n'envoie le token qu'à NOTRE API, jamais à un autre site
  const isApiCall = req.url.startsWith(environment.apiUrl);

  const authReq = token && isApiCall
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })   // copie de la requête + en-tête
    : req;

  return next(authReq).pipe(                     // next = envoie la requête (à l'intercepteur suivant ou au réseau)
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401 && token) {       // token expiré ou invalide
        auth.logout();                           // → retour à la page de login
      }
      return throwError(() => error);            // on laisse l'erreur continuer vers le composant
    })
  );
};