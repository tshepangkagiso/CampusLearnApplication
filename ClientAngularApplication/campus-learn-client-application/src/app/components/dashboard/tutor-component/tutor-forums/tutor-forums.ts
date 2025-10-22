import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';

@Component({
  selector: 'app-tutor-forums',
  imports: [RouterLink],
  templateUrl: './tutor-forums.html',
  styleUrl: './tutor-forums.css'
})
export class TutorForums {
  private sessionUser = inject(AuthService);
  public currentLoggedInUser ?: AuthSessionUser
  private tutorService = inject(TutorService);
  
  ngOnInit(): void {
    let user = this.sessionUser.getUser();
    if(user?.userProfileID)
    {
      this.currentLoggedInUser = user;
    }
    else
    {
      console.log("empty student number: " + user?.userProfileID)
    }
  }

  



  onLogout() {
    this.sessionUser.onLogout();
  }
}
