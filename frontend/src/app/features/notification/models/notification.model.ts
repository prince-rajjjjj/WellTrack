export interface Notification {
  id: string;
  userID: string;
  programID?: string;
  message: string;
  category: string;
  status: 'Active' | 'Inactive';
  createdAt: string;
}