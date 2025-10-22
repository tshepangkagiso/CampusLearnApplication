import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { StudentService } from '../../../../services/Student Related Services/student.service';

@Component({
  selector: 'app-student-faq',
  imports: [RouterLink],
  templateUrl: './student-faq.html',
  styleUrl: './student-faq.css'
})
export class StudentFaq {
  private sessionUser = inject(AuthService);
  public currentLoggedInUser ?: AuthSessionUser

  private studentService = inject(StudentService);

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
