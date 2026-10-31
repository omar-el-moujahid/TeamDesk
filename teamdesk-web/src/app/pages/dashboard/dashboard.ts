import { Component, OnInit, inject, signal } from '@angular/core';
import { RequestsService } from '../../services/requests.service';
import { DashboardDto } from '../../models/request.model';

@Component({
  selector: 'app-dashboard',
  imports: [],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class Dashboard implements OnInit {
  private requestsService = inject(RequestsService);

  data = signal<DashboardDto | null>(null);
  error = signal<string | null>(null);

  ngOnInit(): void {
    this.requestsService.getDashboard().subscribe({
      next: d => this.data.set(d),
      error: () => this.error.set('Impossible de charger le tableau de bord.')
    });
  }

  // Pourcentage d'un statut, pour la largeur de la barre
  percent(count: number): number {
    const total = this.data()?.total ?? 0;
    return total === 0 ? 0 : Math.round((count * 100) / total);
  }
}