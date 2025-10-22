import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';

@Component({
  selector: 'app-tutor-faq',
  imports: [RouterLink],
  templateUrl: './tutor-faq.html',
  styleUrl: './tutor-faq.css'
})
export class TutorFaq {
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
