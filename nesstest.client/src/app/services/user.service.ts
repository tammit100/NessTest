import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class UserService {
  private baseApiUrl = 'https://localhost:7237/api'; 
  private endpoints = {
    users: 'Users',
    roles: 'Roles'
  };
  selectedUser: any = null;                   

  constructor(private http: HttpClient) {}
  // משתמשים
  getUsers(): Observable<any[]> {
    return this.http.get<any[]>('Users');
  }

  // עדכון
  updateUser(user: any): Observable<any> {
    return this.http.put('Users/' + user.id, user);
  }
  
  // תפקידים
  getRoles(): Observable<any[]> {
    return this.http.get<any[]>('Roles');
  }

  // הוספת משתמש
  addUser(user: any): Observable<any> {
    return this.http.post('Users', user);
  }
}
