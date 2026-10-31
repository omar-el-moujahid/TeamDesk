import { Injectable, computed, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.model';

const STORAGE_KEY = 'teamdesk_auth';            // la clé utilisée dans le localStorage

@Injectable({ providedIn: 'root' })              // une seule instance partagée par toute l'app
export class AuthService {
  private http = inject(HttpClient);
  private router = inject(Router);

  // L'utilisateur connecté. Au démarrage, on relit le localStorage :
  // si on actualise la page (F5), on reste connecté.
  private currentUser = signal<LoginResponse | null>(this.loadFromStorage());

  readonly user = this.currentUser.asReadonly();                            // lecture seule pour l'extérieur
  readonly isLoggedIn = computed(() => this.currentUser() !== null);        // recalculé automatiquement
  readonly isManager = computed(() => this.currentUser()?.role === 'Manager');

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        tap(response => {                        // tap = "fais quelque chose au passage", sans modifier la réponse
          localStorage.setItem(STORAGE_KEY, JSON.stringify(response));
          this.currentUser.set(response);
        })
      );
  }

  logout(): void {
    localStorage.removeItem(STORAGE_KEY);        // côté client, se déconnecter = oublier le token
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  private loadFromStorage(): LoginResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? (JSON.parse(raw) as LoginResponse) : null;
  }
}