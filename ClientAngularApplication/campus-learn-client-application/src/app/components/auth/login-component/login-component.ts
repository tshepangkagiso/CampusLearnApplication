import { Component, inject } from '@angular/core';
import { AuthLogin } from '../../../Interfaces/auth/auth-login';
import { AuthService } from '../../../services/auth/auth-service';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login-component',
  imports: [FormsModule],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css'
})
export class LoginComponent 
{
  private authService = inject(AuthService);
  public message = ''
  public error = false;
  private router = inject(Router);
  public user : AuthLogin = {
    email: '',
    password: ''
  }

  onSubmit() {
    this.message = '';
    if (this.user.email === '' || this.user.password === '') {
      this.message = 'empty fields';
      this.error = true;
    } else {
      this.authService.onLogin(this.user).subscribe({
        next: (res) => {
          sessionStorage.setItem('token', res.token);
          this.error = false;
          this.router.navigate(['/temp']);
        },
        error: (err) => {
          this.message = 'Invalid credentials';
          this.error = true;
        }
      });
    }
  }



}
