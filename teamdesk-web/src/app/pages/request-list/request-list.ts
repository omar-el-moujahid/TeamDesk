import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { RequestsService } from '../../services/requests.service';
import { RequestListItem, RequestStatus } from '../../models/request.model';

@Component({
  selector: 'app-request-list',
  imports: [DatePipe, RouterLink],
  templateUrl: './request-list.html',
  styleUrl: './request-list.css'
})
export class RequestList implements OnInit {
  private requestsService = inject(RequestsService);

  readonly statuses: RequestStatus[] = ['New', 'InProgress', 'Resolved', 'Closed']; // ← tes valeurs d'enum
  readonly pageSize = 10;

  requests = signal<RequestListItem[]>([]);
  totalCount = signal(0);
  page = signal(1);
  status = signal<RequestStatus | ''>('');          // '' = tous les statuts
  loading = signal(true);
  error = signal<string | null>(null);

  // Nombre de pages, recalculé automatiquement quand totalCount change
  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / this.pageSize)));

  ngOnInit(): void {
    this.load();
  }

  onStatusChange(value: string): void {
    this.status.set(value as RequestStatus | '');
    this.page.set(1);                                 // nouveau filtre → retour à la page 1
    this.load();
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages()) return;
    this.page.set(p);
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);

    this.requestsService.getAll(this.page(), this.pageSize, this.status() || undefined).subscribe({
      next: result => {
        this.requests.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Impossible de charger les demandes.');
        this.loading.set(false);
      }
    });
  }
}