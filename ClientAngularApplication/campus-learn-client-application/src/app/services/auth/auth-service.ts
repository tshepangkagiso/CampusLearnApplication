import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthLogin } from '../../Interfaces/auth/auth-login';
import { AuthLoginResponse } from '../../Interfaces/auth/auth-login-response';
import { Router } from '@angular/router';
import { AuthRegister } from '../../Interfaces/auth/auth-register';
import { AuthRegisterResponse } from '../../Interfaces/auth/auth-register-response';
import { AuthSessionUser } from '../../Interfaces/auth/auth-session-user';

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
    sessionStorage.removeItem(this.userKey)
    this.router.navigate(['/login']);
  }

  //registration related: 
  onRegister(user: AuthRegister)
  {
    return this.httpClient.post<AuthRegisterResponse>(`${this.baseUrl}/register`, user);
  }
}
