import { Component, inject } from '@angular/core';
import { AuthLogin } from '../../../Interfaces/auth/auth-login';
import { AuthService } from '../../../services/auth/auth-service';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-login-component',
  imports: [FormsModule, CommonModule,RouterLink],
  templateUrl: './login-component.html',
  styleUrl: './login-component.css'
})
export class LoginComponent 
{
  private authService = inject(AuthService);
  public message = ''
  public error = false;
  private router = inject(Router);
  public loading = false;

  public user : AuthLogin = {
    email: '',
    password: ''
  }


onSubmit() {
  this.message = '';
  this.error = false;

  if (this.user.email === '' || this.user.password === '') {
    this.message = 'empty fields';
    this.error = true;
    return;
  }

  this.loading = true; // ⏳ Start loading

  this.authService.onLogin(this.user).subscribe({
    next: (res) => {
      sessionStorage.setItem('token', res.token);
      sessionStorage.setItem('user', JSON.stringify(res.user))
      this.error = false;
      this.loading = false; // ✅ Stop loading

      let user = this.authService.getUser();

      if(user !== null)
      {
        if(user.userRole === 0)
        {
          this.router.navigate(['/student']);
        }       
        else if(user.userRole === 1)
        {
          this.router.navigate(['/tutor']);
        }
        else if(user.userRole === 2)
        {
          this.router.navigate(['/admin']);
        }
        else
        {
          this.router.navigate(['/temp']);
        }

      }
      
    },
    error: (err) => {
      this.message = 'Invalid credentials';
      this.error = true;
      this.loading = false; // ❌ Stop loading on error
    }
  });
}




}
