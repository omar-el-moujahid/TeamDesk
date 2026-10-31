import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { RequestList } from './pages/request-list/request-list';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'requests', component: RequestList, canActivate: [authGuard] },  // protégée
  { path: '', redirectTo: 'requests', pathMatch: 'full' },
  { path: '**', redirectTo: 'requests' }      // toute URL inconnue → /requests
];