// ⚠️ Mets EXACTEMENT les mêmes valeurs que tes enums C#
export type RequestStatus = 'New' | 'InProgress' | 'Resolved' | 'Closed';
export type Priority = 'Low' | 'Medium' | 'High';

// ----- Liste -----
export interface RequestListItem {
  id: number;
  title: string;
  status: RequestStatus;
  priority: Priority;
  categoryName: string;
  createdByName: string;
  assignedToName: string | null;
  createdAt: string;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}

// ----- Détail -----
export interface CommentDto {
  id: number;
  content: string;
  authorName: string;
  createdAt: string;
}

export interface RequestDetailDto extends RequestListItem {
  description: string;
  comments: CommentDto[];
}

// ----- Création -----
export interface RequestCreateDto {
  title: string;
  description: string;
  categoryId: number;
  priority: Priority;
}

// ----- Listes de référence -----
export interface CategoryDto { id: number; name: string; }
export interface UserDto { id: number; name: string; role: 'User' | 'Manager'; }

// ----- Tableau de bord -----
export interface StatusCountDto { status: RequestStatus; count: number; }
export interface DashboardDto { total: number; byStatus: StatusCountDto[]; }