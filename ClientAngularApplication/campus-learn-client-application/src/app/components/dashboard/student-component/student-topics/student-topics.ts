import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../services/auth/auth-service';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';

@Component({
  selector: 'app-student-topics',
  imports: [RouterLink],
  templateUrl: './student-topics.html',
  styleUrl: './student-topics.css'
})
export class StudentTopics implements OnInit{
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
