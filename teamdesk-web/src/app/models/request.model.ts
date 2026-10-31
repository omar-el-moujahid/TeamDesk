export type RequestStatus = 'New' | 'InProgress' | 'Resolved' | 'Closed'; // ← adapte aux valeurs de TON enum C#
export type Priority = 'Low' | 'Medium' | 'High';                         // ← idem

export interface RequestListItem {
  id: number;
  title: string;
  status: RequestStatus;
  priority: Priority;
  categoryName: string;
  createdByName: string;
  assignedToName: string | null;
  createdAt: string;            // une date arrive en texte dans le JSON
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
}