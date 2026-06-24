import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { tap } from 'rxjs/operators';
import { AuthResponse, User } from '../models/models';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly API = 'http://localhost:5000/api';
  private readonly TOKEN_KEY = 'mw26_token';
  private readonly USER_KEY  = 'mw26_user';

  currentUser = signal<User | null>(this.loadUser());

  constructor(private http: HttpClient, private router: Router) {}

  login(username: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/login`, { username, password })
      .pipe(tap(r => this.saveSession(r)));
  }

  register(username: string, email: string, password: string) {
    return this.http.post<AuthResponse>(`${this.API}/auth/register`, { username, email, password })
      .pipe(tap(r => this.saveSession(r)));
  }

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUser.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAdmin(): boolean {
    return this.currentUser()?.role === 'admin';
  }

  private saveSession(r: AuthResponse) {
    localStorage.setItem(this.TOKEN_KEY, r.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(r.user));
    this.currentUser.set(r.user);
  }

  private loadUser(): User | null {
    const u = localStorage.getItem(this.USER_KEY);
    return u ? JSON.parse(u) : null;
  }
}
