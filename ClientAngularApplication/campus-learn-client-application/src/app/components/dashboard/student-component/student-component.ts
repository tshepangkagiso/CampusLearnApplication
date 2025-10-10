import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { QualificationStringMap } from '../../../Interfaces/enums/qualification';

@Component({
  selector: 'app-student-component',
  imports: [],
  templateUrl: './student-component.html',
  styleUrl: './student-component.css'
})
export class StudentComponent {
  private authService = inject(AuthService);  

  public name : string = ''
  public qualification : string = ''
  
  updateUserInfo() {
    const user = this.authService.getUser();

    if (user) {
      this.name = user.name;
      this.qualification = QualificationStringMap[user.qualification];
    } else {
      this.name = '';
      this.qualification = '';
    }
  }

  ngOnInit() {
  this.updateUserInfo();
  }

  onLogout(){
    this.authService.onLogout();
  }
}
