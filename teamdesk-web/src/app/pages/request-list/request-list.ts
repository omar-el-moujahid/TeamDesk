import { Component, OnInit, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RequestsService } from '../../services/requests.service';
import { RequestListItem } from '../../models/request.model';

@Component({
  selector: 'app-request-list',
  imports: [DatePipe],                         // pour formater les dates dans le template
  templateUrl: './request-list.html',
  styleUrl: './request-list.css'
})
export class RequestList implements OnInit {
  private requestsService = inject(RequestsService);

  requests = signal<RequestListItem[]>([]);    // l'état du composant (comme useState)
  totalCount = signal(0);
  loading = signal(true);
  error = signal<string | null>(null);

  ngOnInit(): void {                           // appelée une fois, à l'affichage du composant
    this.requestsService.getAll().subscribe({  // subscribe = "lance l'appel et préviens-moi"
      next: result => {                        // succès
        this.requests.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: () => {                           // échec (API éteinte, 401, 500...)
        this.error.set('Impossible de charger les demandes.');
        this.loading.set(false);
      }
    });
  }
}