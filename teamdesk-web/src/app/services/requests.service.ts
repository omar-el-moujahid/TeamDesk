import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { PagedResult, RequestListItem, RequestStatus } from '../models/request.model';

@Injectable({ providedIn: 'root' })        // une seule instance pour toute l'application (comme un Singleton)
export class RequestsService {
  private http = inject(HttpClient);       // injection de dépendances
  private baseUrl = `${environment.apiUrl}/requests`;

  getAll(page = 1, pageSize = 10, status?: RequestStatus): Observable<PagedResult<RequestListItem>> {
    let params = new HttpParams()
      .set('page', page)
      .set('pageSize', pageSize);           // → ?page=1&pageSize=10
    if (status) params = params.set('status', status);

    return this.http.get<PagedResult<RequestListItem>>(this.baseUrl, { params });
  }
}