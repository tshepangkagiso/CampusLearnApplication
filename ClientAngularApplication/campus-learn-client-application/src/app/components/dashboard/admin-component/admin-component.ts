import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { UserRoleMap } from '../../../Interfaces/enums/userRole';

@Component({
  selector: 'app-admin-component',
  imports: [],
  templateUrl: './admin-component.html',
  styleUrl: './admin-component.css'
})
export class AdminComponent {
  private authService = inject(AuthService);  

  public name : string = ''
  public ID : number = 0
  public role : number = 0
  
  updateUserInfo() {
    const user = this.authService.getUser();

    if (user) {
      this.name = user.name;
      this.ID = user.userProfileID;
      this.role = user.userRole;
    } else {
      this.name = '';
      this.ID = 0;
      this.role = 0;
    }
  }

  ngOnInit() {
  this.updateUserInfo();
  }

  onLogout(){
    this.authService.onLogout();
  }
}
