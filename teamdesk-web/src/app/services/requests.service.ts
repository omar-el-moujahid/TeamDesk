import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import {
  CommentDto, DashboardDto, PagedResult, RequestCreateDto,
  RequestDetailDto, RequestListItem, RequestStatus
} from '../models/request.model';

@Injectable({ providedIn: 'root' })
export class RequestsService {
  private http = inject(HttpClient);
  private baseUrl = `${environment.apiUrl}/requests`;

  // GET /api/requests?page=&pageSize=&status=
  getAll(page = 1, pageSize = 10, status?: RequestStatus): Observable<PagedResult<RequestListItem>> {
    let params = new HttpParams().set('page', page).set('pageSize', pageSize);
    if (status) params = params.set('status', status);
    return this.http.get<PagedResult<RequestListItem>>(this.baseUrl, { params });
  }

  // GET /api/requests/5
  getById(id: number): Observable<RequestDetailDto> {
    return this.http.get<RequestDetailDto>(`${this.baseUrl}/${id}`);
  }

  // POST /api/requests → 201 + la demande créée
  create(dto: RequestCreateDto): Observable<RequestDetailDto> {
    return this.http.post<RequestDetailDto>(this.baseUrl, dto);
  }

  // PUT /api/requests/5/assign → 204
  assign(id: number, assignedToId: number): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}/assign`, { assignedToId });
  }

  // PATCH /api/requests/5/status → 204
  updateStatus(id: number, status: RequestStatus): Observable<void> {
    return this.http.patch<void>(`${this.baseUrl}/${id}/status`, { status });
  }

  // POST /api/requests/5/comments → 201 + le commentaire
  addComment(id: number, content: string): Observable<CommentDto> {
    return this.http.post<CommentDto>(`${this.baseUrl}/${id}/comments`, { content });
  }

  // GET /api/dashboard (Manager)
  getDashboard(): Observable<DashboardDto> {
    return this.http.get<DashboardDto>(`${environment.apiUrl}/dashboard`);
  }
}