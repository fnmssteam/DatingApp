import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { Photo } from '../_models/photo';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsersWithRoles() {
    return this.http.get<User[]>(this.baseUrl + 'admin/users-with-roles');
  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post<string[]>(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }

  getUnapprovedPhotos(pageNumber: number, pageSize: number) {
    let params = getPaginationHeaders(pageNumber, pageSize);

    return getPaginatedResult<Photo[]>(
      this.baseUrl + 'admin/photos-to-moderate',
      params,
      this.http
    );
  }

  approvePhoto(id: number) {
    return this.http.post(this.baseUrl + 'admin/approve-photo/' + id, {});
  }

  rejectPhoto(id: number) {
    return this.http.post(this.baseUrl + 'admin/reject-photo/' + id, {});
  }
}
