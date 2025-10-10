import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { Router, RouterLink } from '@angular/router';

@Component({
  selector: 'app-restricted-component',
  imports: [RouterLink],
  templateUrl: './restricted-component.html',
  styleUrl: './restricted-component.css'
})
export class RestrictedComponent {
  attemptedPath?: string;

  constructor(private auth: AuthService, private router: Router) {
    const nav = this.router.getCurrentNavigation();
    this.attemptedPath = nav?.extras?.state?.['attemptedPath'];
  }

  onLogout() {
    this.auth.onLogout(); 
  }
}
