import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { RequestsService } from '../../services/requests.service';
import { AuthService } from '../../services/auth.service';
import { LookupService } from '../../services/lookup.service';
import { RequestDetailDto, RequestStatus, UserDto } from '../../models/request.model';

@Component({
  selector: 'app-request-detail',
  imports: [DatePipe, RouterLink, ReactiveFormsModule],
  templateUrl: './request-detail.html',
  styleUrl: './request-detail.css'
})
export class RequestDetail implements OnInit {
  private route = inject(ActivatedRoute);
  private requestsService = inject(RequestsService);
  private auth = inject(AuthService);
  private lookup = inject(LookupService);

  private id = Number(this.route.snapshot.paramMap.get('id'));

  request = signal<RequestDetailDto | null>(null);
  loading = signal(true);
  error = signal<string | null>(null);
  statusMessage = signal<string | null>(null);
  commentError = signal<string | null>(null);

  // ----- Assignation (Manager) -----
  isManager = this.auth.isManager;
  users = signal<UserDto[]>([]);
  assignMessage = signal<string | null>(null);

  readonly statuses: RequestStatus[] = ['New', 'InProgress', 'Resolved', 'Closed'];

  commentControl = new FormControl('', {
    nonNullable: true,
    validators: [Validators.required, Validators.maxLength(1000)]
  });

  ngOnInit(): void {
    this.requestsService.getById(this.id).subscribe({
      next: r => {
        this.request.set(r);
        this.loading.set(false);
      },
      error: (err: HttpErrorResponse) => {
        this.error.set(err.status === 404 ? 'Demande introuvable.' : 'Erreur de chargement.');
        this.loading.set(false);
      }
    });

    if (this.isManager()) {                      // seul un Manager peut appeler GET /api/users
      this.lookup.getUsers().subscribe({
        next: u => this.users.set(u),
        error: () => this.assignMessage.set('Impossible de charger la liste des utilisateurs.')
      });
    }
  }

  changeStatus(select: HTMLSelectElement): void {
    const current = this.request();
    if (!current) return;
    const newStatus = select.value as RequestStatus;
    this.statusMessage.set(null);

    this.requestsService.updateStatus(this.id, newStatus).subscribe({
      next: () => {
        this.request.set({ ...current, status: newStatus });
        this.statusMessage.set('Statut mis à jour.');
      },
      error: (err: HttpErrorResponse) => {
        select.value = current.status;
        this.statusMessage.set(err.status === 403
          ? "Vous n'avez pas le droit de modifier ce statut (Manager ou personne assignée uniquement)."
          : 'Erreur lors de la mise à jour.');
      }
    });
  }

  assign(select: HTMLSelectElement): void {
    const userId = Number(select.value);
    if (!userId) return;
    const user = this.users().find(u => u.id === userId);
    this.assignMessage.set(null);

    this.requestsService.assign(this.id, userId).subscribe({
      next: () => {
        this.request.update(r => r ? { ...r, assignedToName: user?.name ?? null } : r);
        this.assignMessage.set(`Demande assignée à ${user?.name}.`);
      },
      error: () => this.assignMessage.set("Erreur lors de l'assignation.")
    });
  }

  addComment(): void {
    if (this.commentControl.invalid) {
      this.commentControl.markAsTouched();
      return;
    }
    this.commentError.set(null);

    this.requestsService.addComment(this.id, this.commentControl.value).subscribe({
      next: comment => {
        this.request.update(r => r ? { ...r, comments: [...r.comments, comment] } : r);
        this.commentControl.reset();
      },
      error: () => this.commentError.set("Impossible d'ajouter le commentaire.")
    });
  }
}