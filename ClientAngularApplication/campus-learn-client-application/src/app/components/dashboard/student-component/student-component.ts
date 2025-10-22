import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { UpdateStudentProfileRequest, SubscribeModuleRequest } from '../../../models/Student Related Models/student-request.dtos';
import { StudentProfileResponse, AvailableModulesResponse, StudentSubscriptionsResponse } from '../../../models/Student Related Models/student-response.dtos';
import { Qualification, UserRole } from '../../../models/Student Related Models/student.models';
import { StudentService } from '../../../services/Student Related Services/student.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-student-component',
  imports: [FormsModule, CommonModule, RouterLink],
  templateUrl: './student-component.html',
  styleUrl: './student-component.css'
})
export class StudentComponent {
  private studentService = inject(StudentService);
  private sessionUser = inject(AuthService);

  public qualification = Qualification;
  public student? : StudentProfileResponse;
  public role = -1;

  public profilePicture = ""
  public updateStudent: UpdateStudentProfileRequest = {
    userProfileID: 0,
    name: '',
    surname: '',
    email: '',
    qualification: Qualification.DIP, // default value
    studentNumber: 0
  };

  public isUpdating = false;

  public availableModules?: AvailableModulesResponse;
  public subscribedModules?: StudentSubscriptionsResponse;; 
  public unsubscribingModule: string | null = null;


  //code we execute when view is done loading
  ngOnInit(): void {
    let user = this.sessionUser.getUser();


   
    if(user?.userProfileID)
    {

       //set current user
      this.studentService.getStudentProfile(user?.userProfileID).subscribe({
        next: res =>{
          this.student = res;
          this.role = user.userRole;

          this.setUpdateStudent(this.student);
          if(this.student.profilePictureUrl)
          {
            this.profilePicture = `http://localhost:7000/users/userprofile/file/${this.student.profilePictureUrl}`
          }
          else{
            this.profilePicture = ""
          }
        },
        error: err => {console.log("error: " + err)}
      });


      //set all available Modules
      this.studentService.getAvailableModules(user?.userProfileID).subscribe({
        next : res =>{
          this.availableModules = res;
        },
        error: err => {console.log("error: " + err)}
      });



      // Load subscribed modules
      this.loadSubscribedModules(user.userProfileID);
    }
    else{
      console.log("empty student number: " + user?.userProfileID)
    }


  }
  
  //methods for displayin student info user

  getQualificationText(qualification: Qualification): string {
    switch (qualification) {
      case Qualification.DIP: return 'Diploma';
      case Qualification.BIT: return 'BIT';
      case Qualification.BCOM: return 'BCom';
      default: return 'Unknown';
    }
  }

  getUserRoleText(userRole: UserRole): string {
    switch (userRole) {
      case UserRole.Student: return 'Student';
      case UserRole.Tutor: return 'Tutor';
      case UserRole.Admin: return 'Admin';
      default: return 'Unknown';
    }
  }

  formatDate(date: Date | string | undefined): string {
    if (!date) return 'Never';
    
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  private refreshStudentData() {
    let user = this.sessionUser.getUser();
    if(user?.userProfileID){
      this.studentService.getStudentProfile(user.userProfileID).subscribe({
        next: res => {
          this.student = res;
        },
        error: err => console.log("Error refreshing: ", err)
      });
    }
  }



  //code for updating user
  setUpdateStudent(student: StudentProfileResponse){
    this.updateStudent.userProfileID = student.userProfileID;
    this.updateStudent.name = student.name;
    this.updateStudent.surname = student.surname;
    this.updateStudent.email = student.email;
    this.updateStudent.qualification = student.qualification;
    this.updateStudent.studentNumber = student.studentNumber;
  }

  onFileSelected(event: any) {
      const file = event.target.files[0];
      if (file) {
        this.updateStudent.profilePicture = file;
      }
    }

  onSubmit() {
    this.isUpdating = true; 
    
    this.studentService.updateStudentProfile(this.updateStudent).subscribe({
      next: res => {
        console.log("Update successful: ", res);
        this.isUpdating = false;
        this.refreshStudentDataWithUIUpdate();
      },
      error: err => {
        console.log("Update error: ", err);
        this.isUpdating = false; 
      }
    });

    this.refreshStudentData();

  }

  private refreshStudentDataWithUIUpdate() {
    let user = this.sessionUser.getUser();
    if(user?.userProfileID){
      this.studentService.getStudentProfile(user.userProfileID).subscribe({
        next: res => {
          this.student = res;
          this.role = user.userRole;
          this.setUpdateStudent(this.student);
          
          // Update profile picture URL
          if(this.student.profilePictureUrl) {
            this.profilePicture = `http://localhost:7000/users/userprofile/file/${this.student.profilePictureUrl}`;
          } else {
            this.profilePicture = "";
          }
          


          console.log("Profile data refreshed successfully");
        },
        error: err => { 
          console.log("Error refreshing student data: ", err);
        }
      });
    }
  }






  // Subscriptions related

  subscribeToModule(moduleCode: string) {
    console.log('Subscribing to module:', moduleCode);
    let user = this.sessionUser.getUser();
    if(user?.userProfileID) {
      let sub: SubscribeModuleRequest = {
        userId: user.userProfileID,
        moduleCode: moduleCode
      };
      
      this.studentService.subscribeToModule(sub).subscribe({
        next: res => {
          console.log('Subscription successful:', res);
          this.refreshAllModules(); // Refresh both lists after subscription
        },
        error: err => {
          console.log('Subscription error:', err);
        }
      });
    }
  }

  unsubscribeFromModule(moduleCode: string) {
    let user = this.sessionUser.getUser();
    
    if(user?.userProfileID) {
      let unsub: SubscribeModuleRequest = {
        userId: user.userProfileID,
        moduleCode: moduleCode
      };
      
      this.studentService.unsubscribeFromModule(unsub).subscribe({
        next: res => {
          this.refreshAllModules(); // Refresh both lists after unsubscription
        },
        error: err => {
          console.log('Unsubscription error:', err);
        }
      });
    }
  }

  private refreshAllModules() {
    let user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      // Refresh available modules
      this.refreshAvailableModules();
      
      // Refresh subscribed modules
      this.loadSubscribedModules(user.userProfileID);
    }
  }

  private refreshAvailableModules() {
    let user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.studentService.getAvailableModules(user.userProfileID).subscribe({
        next: res => {
          this.availableModules = res;
        },
        error: err => { console.log("Error refreshing modules: ", err) }
      });
    }
  }

  loadSubscribedModules(userId: number) {
    this.studentService.getSubscribedModules(userId).subscribe({
      next: res => {
        this.subscribedModules = res;
      },
      error: err => { console.log("error getting subscribed modules: ", err) }
    });
  }


  //logout
  onLogout(){
    this.sessionUser.onLogout();
  }
}
