import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CategoryDto, UserDto } from '../models/request.model';

@Injectable({ providedIn: 'root' })
export class LookupService {
  private http = inject(HttpClient);

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(`${environment.apiUrl}/categories`);
  }

  getUsers(): Observable<UserDto[]> {
    return this.http.get<UserDto[]>(`${environment.apiUrl}/users`);
  }
}