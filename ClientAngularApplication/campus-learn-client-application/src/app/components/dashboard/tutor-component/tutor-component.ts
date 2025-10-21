import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { Qualification, UserRole } from '../../../models/Student Related Models/student.models';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AvailableTutorModulesResponse, TutorProfileResponse, TutorQualificationsResponse } from '../../../models/Tutor Related Models/tutor-response.dtos';
import { SubscribeModuleRequest, UpdateTutorProfileRequest } from '../../../models/Tutor Related Models/tutor-request.dtos';
import { TutorService } from '../../../services/Tutor Related Services/tutor.service';

@Component({
  selector: 'app-tutor-component',
  imports: [FormsModule, CommonModule],
  templateUrl: './tutor-component.html',
  styleUrl: './tutor-component.css'
})
export class TutorComponent implements OnInit {
  private tutorService = inject(TutorService);
  private sessionUser = inject(AuthService);

  public qualification = Qualification;
  public tutor?: TutorProfileResponse;

  public role = -1;

  public profilePicture = ""
  public updateTutor: UpdateTutorProfileRequest = {
    userProfileID: 0,
    name: '',
    surname: '',
    email: '',
    qualification: Qualification.DIP, // default value
    studentNumber: 0
  };

  public isUpdating = false;

  public availableModules?: AvailableTutorModulesResponse;
  public qualifiedModules?: TutorQualificationsResponse; 
  public unqualifyingModule: string | null = null;

  //code we execute when view is done loading
  ngOnInit(): void {
    let user = this.sessionUser.getUser();
   
    if(user?.userProfileID) 
    {

      //set current tutor
      this.tutorService.getTutorProfile(user.userProfileID).subscribe({
        next: res => {
          this.tutor = res;
          this.role = user.userRole;

          this.setUpdateTutor(this.tutor);
          
          if(this.tutor.profilePictureUrl) {
            this.profilePicture = `http://localhost:7000/users/userprofile/file/${this.tutor.profilePictureUrl}`
          } else {
            this.profilePicture = ""
          }

        console.log("user id:", this.tutor.email)


        },
        error: err => {console.log("error: ", err)}
      });
      


      //set all available modules to qualify for
      this.tutorService.getAvailableModules(user.userProfileID).subscribe({
        next: res => {
          this.availableModules = res;
        },
        error: err => {console.log("error: ", err)}
      });


      // Load qualified modules
      this.loadQualifiedModules(user.userProfileID);



    } else {
      console.log("empty tutor number: ", user?.userProfileID)
    }
  }
  
  //methods for displaying tutor info
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

  private refreshTutorData() {
    let user = this.sessionUser.getUser();
    if(user?.userProfileID) {
      this.tutorService.getTutorProfile(user.userProfileID).subscribe({
        next: res => {
          this.tutor = res;
        },
        error: err => console.log("Error refreshing: ", err)
      });
    }
  }

  //code for updating tutor
  setUpdateTutor(tutor: TutorProfileResponse) {
    this.updateTutor.userProfileID = tutor.userProfileID;
    this.updateTutor.name = tutor.name;
    this.updateTutor.surname = tutor.surname;
    this.updateTutor.email = tutor.email;
    this.updateTutor.qualification = tutor.qualification;
    this.updateTutor.studentNumber = tutor.studentNumber;
  }

  onFileSelected(event: any) {
    const file = event.target.files[0];
    if (file) {
      this.updateTutor.profilePicture = file;
    }
  }

  onSubmit() {
    this.isUpdating = true; 
    
    this.tutorService.updateTutorProfile(this.updateTutor).subscribe({
      next: res => {
        console.log("Update successful: ", res);
        this.isUpdating = false;
        this.refreshTutorDataWithUIUpdate();
      },
      error: err => {
        console.log("Update error: ", err);
        this.isUpdating = false; 
      }
    });
  }

  private refreshTutorDataWithUIUpdate() {
    let user = this.sessionUser.getUser();
    if(user?.userProfileID) {
      this.tutorService.getTutorProfile(user.userProfileID).subscribe({
        next: res => {
          this.tutor = res;
          this.role = user.userRole;
          this.setUpdateTutor(this.tutor);
          
          // Update profile picture URL
          if(this.tutor.profilePictureUrl) {
            this.profilePicture = `http://localhost:7000/users/userprofile/file/${this.tutor.profilePictureUrl}`;
          } else {
            this.profilePicture = "";
          }
          
          console.log("Profile data refreshed successfully");
        },
        error: err => { 
          console.log("Error refreshing tutor data: ", err);
        }
      });
    }
  }

  // Qualifications related
  qualifyForModule(moduleCode: string) {
    console.log('Qualifying for module:', moduleCode);
    let user = this.sessionUser.getUser();
    if(user?.userProfileID) {
      let sub: SubscribeModuleRequest = {
        userId: user.userProfileID,
        moduleCode: moduleCode
      };
      
      this.tutorService.qualifyForModule(sub).subscribe({
        next: res => {
          console.log('Qualification successful:', res);
          this.refreshAllModules();
        },
        error: err => {
          console.log('Qualification error:', err);
        }
      });
    }
  }

  removeQualification(moduleCode: string) {
    let user = this.sessionUser.getUser();
    
    if(user?.userProfileID) {
      let unsub: SubscribeModuleRequest = {
        userId: user.userProfileID,
        moduleCode: moduleCode
      };
      
      this.tutorService.removeQualification(unsub).subscribe({
        next: res => {
          this.refreshAllModules();
        },
        error: err => {
          console.log('Remove qualification error:', err);
        }
      });
    }
  }

  private refreshAllModules() {
    let user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      // Refresh available modules
      this.refreshAvailableModules();
      
      // Refresh qualified modules
      this.loadQualifiedModules(user.userProfileID);
    }
  }

  private refreshAvailableModules() {
    let user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.tutorService.getAvailableModules(user.userProfileID).subscribe({
        next: res => {
          this.availableModules = res;
        },
        error: err => { console.log("Error refreshing modules: ", err) }
      });
    }
  }

  loadQualifiedModules(userId: number) {
    this.tutorService.getQualifiedModules(userId).subscribe({
      next: res => {
        this.qualifiedModules = res;
      },
      error: err => { console.log("error getting qualified modules: ", err) }
    });
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}