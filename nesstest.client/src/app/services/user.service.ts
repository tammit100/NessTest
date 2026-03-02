import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../Models/User.model';
import { Role } from '../Models/Role.model';

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
  getUsers(): Observable<User[]> {
    return this.http.get<User[]>('Users');
  }

  // עדכון
  updateUser(user: User): Observable<User> {
    return this.http.put<User>('Users/' + user.id, user);
  }
  
  // תפקידים
  getRoles(): Observable<Role[]> {
    return this.http.get<Role[]>('Roles');
  }

  // הוספת משתמש
  addUser(user: User): Observable<User> {
    return this.http.post<User>('Users', user);
  }
}
