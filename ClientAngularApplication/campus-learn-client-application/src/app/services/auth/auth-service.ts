import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { AuthLogin } from '../../Interfaces/auth/auth-login';
import { AuthLoginResponse } from '../../Interfaces/auth/auth-login-response';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private httpClient  = inject(HttpClient);
  private baseUrl = 'http://localhost:7000/authentication/auth';
  private router = inject(Router);
  private tokenKey = 'token'

  //login related:
  onLogin(user: AuthLogin) {
    return this.httpClient.post<AuthLoginResponse>(`${this.baseUrl}/login`, user);
  }


  isLoggedIn()
  {
    return !!sessionStorage.getItem(this.tokenKey)
  }

  getItem(){
    return sessionStorage.getItem(this.tokenKey)
  }

  onLogout(){
    sessionStorage.removeItem(this.tokenKey);
    this.router.navigate(['/login']);
  }

  //registration related: 
  onRegister()
  {
    
  }
}
