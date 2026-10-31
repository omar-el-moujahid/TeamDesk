import { Routes } from '@angular/router';
import { Login } from './pages/login/login';
import { RequestList } from './pages/request-list/request-list';
import { RequestCreate } from './pages/request-create/request-create';
import { RequestDetail } from './pages/request-detail/request-detail';
import { Dashboard } from './pages/dashboard/dashboard';
import { authGuard, managerGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: Login },
  { path: 'requests', component: RequestList, canActivate: [authGuard] },
  { path: 'requests/new', component: RequestCreate, canActivate: [authGuard] },   // AVANT :id
  { path: 'requests/:id', component: RequestDetail, canActivate: [authGuard] },
  { path: 'dashboard', component: Dashboard, canActivate: [managerGuard] },
  { path: '', redirectTo: 'requests', pathMatch: 'full' },
  { path: '**', redirectTo: 'requests' }                                          // TOUJOURS en dernier
];