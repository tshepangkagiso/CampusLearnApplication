import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthLogin } from '../../Interfaces/auth/auth-login';
import { AuthLoginResponse } from '../../Interfaces/auth/auth-login-response';
import { Router } from '@angular/router';
import { AuthRegister } from '../../Interfaces/auth/auth-register';
import { AuthRegisterResponse } from '../../Interfaces/auth/auth-register-response';
import { AuthSessionUser } from '../../Interfaces/auth/auth-session-user';
import { ChangePasswordRequest } from '../../models/Student Related Models/student-request.dtos';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private httpClient  = inject(HttpClient);
  private baseUrl = 'http://localhost:7000/authentication/auth';
  private router = inject(Router);
  private tokenKey = 'token'
  private userKey = 'user'

  //login related:
  onLogin(user: AuthLogin) {
   return this.httpClient.post<AuthLoginResponse>(`${this.baseUrl}/login`, user);
  }


  isLoggedIn()
  {
    return !!sessionStorage.getItem(this.tokenKey)
  }


    // Change password
    changePassword(request: ChangePasswordRequest): Observable<any> {
      return this.httpClient.post(`${this.baseUrl}/change-password`, request);
    }

    // Request password reset
    requestPasswordReset(email: string): Observable<any> {
      return this.httpClient.post(`${this.baseUrl}/reset-password`, { email });
    }

    // Validate token
    validateToken(token: string): Observable<any> {
      return this.httpClient.post(`${this.baseUrl}/validate-token`, { token });
    }

    
    // jwt token in session cookie
    getItem(){
      return sessionStorage.getItem(this.tokenKey)
    }

  //actual user who logged in, has all details we need
  getUser(): AuthSessionUser | null{
    const user = sessionStorage.getItem(this.userKey);
    console.log(user)
    return user ? JSON.parse(user) : null;
  }

  onLogout(){
    sessionStorage.removeItem(this.tokenKey);
    sessionStorage.removeItem(this.userKey);
    sessionStorage.clear();
    this.router.navigate(['/login']);
  }

  //registration related: 
  onRegister(user: AuthRegister)
  {
    return this.httpClient.post<AuthRegisterResponse>(`${this.baseUrl}/register`, user);
  }

  getToken(): string {
    return localStorage.getItem('token') || '';
  }
}
