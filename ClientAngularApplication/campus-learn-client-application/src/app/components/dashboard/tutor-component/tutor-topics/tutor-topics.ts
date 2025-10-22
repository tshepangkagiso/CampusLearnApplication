import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';

@Component({
  selector: 'app-tutor-topics',
  imports: [RouterLink],
  templateUrl: './tutor-topics.html',
  styleUrl: './tutor-topics.css'
})
export class TutorTopics {
  private sessionUser = inject(AuthService);
  public currentLoggedInUser ?: AuthSessionUser

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
