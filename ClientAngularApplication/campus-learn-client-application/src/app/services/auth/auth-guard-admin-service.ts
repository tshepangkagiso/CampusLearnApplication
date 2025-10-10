import { inject, Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from './auth-service';
import { UserRoleMap, UserRoleNumber, UserRoleString } from '../../Interfaces/enums/userRole';


@Injectable({
  providedIn: 'root'
})
export class AuthGuardAdminService implements CanActivate {
  private router = inject(Router);
  private authService = inject(AuthService);


  //if not logged in canActivate denies entry to protect routes 
  canActivate()
  {
    let sessionUser = this.authService.getUser();
    if( sessionUser === null)
    {
      this.router.navigate(['/restricted']);
      return false;
    }
    if(UserRoleMap[sessionUser.userRole] === UserRoleString.Admin)
    {
      return true;
    }
    else{
      this.router.navigate(['/restricted']); //come here if isLoggedIn() is false;
      return false;
    }
  }

}
