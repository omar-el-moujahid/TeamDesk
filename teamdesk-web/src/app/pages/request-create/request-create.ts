import { Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { RequestsService } from '../../services/requests.service';
import { LookupService } from '../../services/lookup.service';
import { CategoryDto, Priority } from '../../models/request.model';

@Component({
  selector: 'app-request-create',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './request-create.html',
  styleUrl: './request-create.css'
})
export class RequestCreate implements OnInit {
  private fb = inject(FormBuilder);
  private requestsService = inject(RequestsService);
  private lookup = inject(LookupService);
  private router = inject(Router);

  categories = signal<CategoryDto[]>([]);
  submitting = signal(false);
  error = signal<string | null>(null);

  readonly priorities: Priority[] = ['Low', 'Medium', 'High'];   // ← tes valeurs d'enum

  // Mêmes règles que les attributs du DTO C# (Required, MaxLength(200), Range(1, ...))
  form = this.fb.nonNullable.group({
    title: ['', [Validators.required, Validators.maxLength(200)]],
    description: ['', Validators.required],
    categoryId: [0, Validators.min(1)],              // 0 = "aucune catégorie choisie"
    priority: ['Medium' as Priority, Validators.required]
  });

  ngOnInit(): void {
    this.lookup.getCategories().subscribe(c => this.categories.set(c));
  }

  // Petit raccourci pour le template : le champ a-t-il une erreur à afficher ?
  invalid(field: 'title' | 'description' | 'categoryId'): boolean {
    const control = this.form.controls[field];
    return control.touched && control.invalid;
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    this.submitting.set(true);
    this.error.set(null);

    this.requestsService.create(this.form.getRawValue()).subscribe({
      next: created => this.router.navigate(['/requests', created.id]),   // → page détail de la nouvelle demande
      error: () => {
        this.error.set('Impossible de créer la demande.');
        this.submitting.set(false);
      }
    });
  }
}