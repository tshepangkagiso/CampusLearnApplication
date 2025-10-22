import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { StudentService } from '../../../../services/Student Related Services/student.service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';
import { FAQResponse } from '../../../../models/Tutor Related Models/tutor-response.dtos';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-student-faq',
  imports: [RouterLink,CommonModule],
  templateUrl: './student-faq.html',
  styleUrl: './student-faq.css'
})
export class StudentFaq {
 private sessionUser = inject(AuthService);
  private studentService = inject(StudentService);
  private tutorService = inject(TutorService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public studentId: number = 0;
  public subscribedModules: any[] = [];
  public faqs: FAQResponse[] = [];
  public isLoading = false;
  public selectedModule: string = '';

  ngOnInit(): void {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      this.loadStudentData(user.userProfileID);
    } else {
      console.log("empty student number: " + user?.userProfileID);
    }
  }

  loadStudentData(userId: number) {
    this.studentService.getStudentIdByUserId(userId).subscribe({
      next: (res) => {
        this.studentId = res.studentID;
        this.loadSubscribedModules(this.studentId);
      },
      error: (err) => {
        console.log("Error getting student ID: ", err);
      }
    });
  }

  loadSubscribedModules(studentId: number) {
    this.isLoading = true;
    this.studentService.getSubscribedModules(studentId).subscribe({
      next: (res) => {
        this.subscribedModules = res.subscribedModules || [];
        this.isLoading = false;
        
        // Load FAQs for the first module by default
        if (this.subscribedModules.length > 0) {
          this.selectedModule = this.subscribedModules[0].moduleCode;
          this.loadFAQsForModule(this.selectedModule);
        }
      },
      error: (err) => {
        console.log("Error loading subscribed modules: ", err);
        this.isLoading = false;
      }
    });
  }

  loadFAQsForModule(moduleCode: string) {
    this.isLoading = true;
    this.selectedModule = moduleCode;
    
    this.tutorService.getModuleFAQs(moduleCode).subscribe({
      next: (res: any) => {
        // Handle the API response structure
        if (Array.isArray(res)) {
          this.faqs = res;
        } else if (res && Array.isArray(res.FAQs)) {
          this.faqs = res.FAQs;
        } else if (res && Array.isArray(res.faqs)) {
          this.faqs = res.faqs;
        } else {
          this.faqs = [];
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading FAQs for module: ", err);
        this.faqs = [];
        this.isLoading = false;
      }
    });
  }

  onModuleChange(event: any) {
    const moduleCode = event.target.value;
    if (moduleCode) {
      this.loadFAQsForModule(moduleCode);
    }
  }

  formatDate(date: Date | string): string {
    if (!date) return 'Unknown';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}
