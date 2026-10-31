import { Component, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  imports: [ReactiveFormsModule],        // nécessaire pour [formGroup] et formControlName
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  error = signal<string | null>(null);
  submitting = signal(false);

  // Le formulaire est défini dans le TypeScript (= Reactive Form)
  form = this.fb.nonNullable.group({     // nonNullable : les champs valent '' au lieu de null
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();      // affiche les messages d'erreur de tous les champs
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    this.auth.login(this.form.getRawValue()).subscribe({
      next: () => this.router.navigate(['/requests']),
      error: (err: HttpErrorResponse) => {
        this.error.set(err.status === 401
          ? 'Email ou mot de passe incorrect.'
          : 'Erreur serveur, réessayez plus tard.');
        this.submitting.set(false);
      }
    });
  }
}