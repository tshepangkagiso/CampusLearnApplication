import { Component, inject } from '@angular/core';
import { AuthRegister } from '../../../Interfaces/auth/auth-register';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../services/auth/auth-service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-registration-component',
  imports: [FormsModule, CommonModule,RouterLink],
  templateUrl: './registration-component.html',
  styleUrl: './registration-component.css'
})
export class RegistrationComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  public user: AuthRegister = {
    name: '',
    surname: '',
    email: '',
    password: '',
    userRole: 0,
    qualification: 0,
    studentNumber: 0
  };

  public loading = false;
  public message = '';
  public error = false;

  onSubmit() {
    this.message = '';
    this.error = false;
    this.loading = true;

    this.authService.onRegister(this.user).subscribe({
      next: (res) => {
        this.loading = false;
        this.error = false;
        this.message = res.Message;
        setTimeout(() => this.router.navigate(['/login']), 1500);
      },
      error: (err) => {
        this.loading = false;
        this.error = true;
        this.message = 'Registration failed. Please try again.';
        console.error(err);
      }
    });
  }

}
