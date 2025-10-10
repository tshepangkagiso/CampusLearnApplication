import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, GuardResult, MaybeAsync, Router, RouterStateSnapshot } from '@angular/router';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService implements CanActivate {
  private router = inject(Router);
  private authService = inject(AuthService);


  //if not logged in canActivate denies entry to protect routes 
  canActivate(){
      if(this.authService.isLoggedIn())
      {
        return true;
      }
      else{
        this.router.navigate(['/login']); //come here if isLoggedIn() is false;
        return false;
      }
  }
}
