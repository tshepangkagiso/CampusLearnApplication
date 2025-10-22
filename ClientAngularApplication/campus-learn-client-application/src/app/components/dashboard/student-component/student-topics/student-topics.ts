import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../../../services/auth/auth-service';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { StudentService } from '../../../../services/Student Related Services/student.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateQueryRequest, CreateStudentResponseRequest } from '../../../../models/Topics Related Models/topics-request.dtos';
import { StudentQueriesResponse, QueryTopicResponse } from '../../../../models/Topics Related Models/topics-response.dtos';
import { AssignedTutor } from '../../../../models/Topics Related Models/topics.models';
import { StudentProfileResponse } from '../../../../models/Student Related Models/student-response.dtos';
import { environment } from '../../../../models/environments/environment';

@Component({
  selector: 'app-student-topics',
  imports: [RouterLink,FormsModule,CommonModule],
  templateUrl: './student-topics.html',
  styleUrl: './student-topics.css'
})
export class StudentTopics implements OnInit {
  private sessionUser = inject(AuthService);
  private studentService = inject(StudentService);
  public student? : StudentProfileResponse;
  
  public currentLoggedInUser?: AuthSessionUser;
  public studentQueries!: StudentQueriesResponse;
  public subscribedModules: any[] = [];
  
  // UI State
  public showCreateForm = false;
  public isLoading = false;
  public isCreating = false;
  public isAssigningTutor = false;
  public selectedQuery: QueryTopicResponse | null = null;
  public isAddingResponse = false;
  
  // Form Models
  public newQuery: CreateQueryRequest = {
    title: '',
    description: '',
    moduleCode: '',
    studentId: 0
  };
  
  public newResponse: CreateStudentResponseRequest = {
    comment: ''
  };

  public assignedTutor!: AssignedTutor;

ngOnInit(): void {
  let user = this.sessionUser.getUser();
  if (user?.userProfileID) {
    this.currentLoggedInUser = user;
    
    // Get student ID first, then load data
    this.studentService.getStudentIdByUserId(user.userProfileID).subscribe({
      next: (res) => { 
        console.log('Student ID:', res.studentID);
        const studentId = res.studentID;
        
        // Load queries with the actual studentId
        this.loadStudentQueries(studentId);
        
        // Load other data
        this.loadSubscribedModules(user.userProfileID);
      },
      error: (err) => {
        console.log("Error getting student ID: ", err);
      }
    });

  } else {
    console.log("empty student number: " + user?.userProfileID);
  }
}



  // Helper method to show success message
  private showAssignmentSuccess(tutor: any) {
    const message = `Query created successfully!\n\nAssigned to tutor: ${tutor.name} ${tutor.surname}\nThey will respond to your query soon.`;
    alert(message);
  }


  createQueryWithTutorAssignment() {
    if (!this.currentLoggedInUser?.userProfileID) return;
    
    if (!this.newQuery.title || !this.newQuery.description || !this.newQuery.moduleCode) {
      alert('Please fill in all required fields');
      return;
    }

    this.isCreating = true;
    this.isAssigningTutor = true;

    // First get tutor, then create query
    this.studentService.getRandomTutorForModule(this.newQuery.moduleCode).subscribe({
      next: (tutorResponse) => {
        const assignedTutorId = tutorResponse.tutor.tutorID;
        this.assignedTutor = tutorResponse.tutor;

        // Create the query - check if your backend expects assignedTutorId
        const createQueryRequest: any = {
          title: this.newQuery.title,
          description: this.newQuery.description,
          moduleCode: this.newQuery.moduleCode,
          studentId: this.currentLoggedInUser!.userProfileID,
          assignedTutorId: assignedTutorId // Include if backend expects it
        };

        this.studentService.createQuery(createQueryRequest, assignedTutorId).subscribe({
          next: (res) => {
            console.log("Query created with tutor assignment: ", res);
            this.handleSuccess(tutorResponse.tutor);
          },
          error: (err) => {
            this.handleError(err);
          }
        });
      },
      error: (err) => {
        this.handleError(err);
      }
    });
  }

  private handleSuccess(tutor: any) {
    this.isCreating = false;
    this.isAssigningTutor = false;
    this.showCreateForm = false;
    this.resetNewQueryForm();
    
    // Reload queries with the correct student ID
    this.studentService.getStudentIdByUserId(this.currentLoggedInUser!.userProfileID!).subscribe({
      next: (res) => {
        this.loadStudentQueries(res.studentID);
        this.showAssignmentSuccess(tutor);
      },
      error: (err) => {
        console.log("Error getting student ID for reload: ", err);
      }
    });
  }

  private handleError(err: any) {
    console.log("Error: ", err);
    this.isCreating = false;
    this.isAssigningTutor = false;
    
    if (err.status === 404) {
      alert('No tutors available for this module. Please try another module.');
    } else {
      alert('Error creating query. Please try again.');
    }
  }


  loadStudentQueries(studentId: number) {
    this.isLoading = true;

    this.studentService.getStudentQueries(studentId).subscribe({
      next: (res) => {
        this.studentQueries = res;
        console.log("Queries: " , this.studentQueries)
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading queries: ", err);
        this.isLoading = false;
      }
    });
  }

  loadSubscribedModules(studentId: number) {
    this.studentService.getSubscribedModules(studentId).subscribe({
      next: (res) => {
        console.log("Subscribed modules loaded: ", res);
        this.subscribedModules = res.subscribedModules || [];
      },
      error: (err) => {
        console.log("Error loading subscribed modules: ", err);
        this.subscribedModules = [];
      }
    });
  }

  private resetNewQueryForm() {
    this.newQuery = {
      title: '',
      description: '',
      moduleCode: '',
      studentId: 0
    };
    this.assignedTutor = {
        message:"",
        tutor: {
          tutorID: 0,
          name: "",
          surname: "",
          email: "",
          qualifiedSince: ""
        }
    }
    
  }


  viewQueryDetails(query: any) {
    this.selectedQuery = {
      ...query,
      responses: this.getResponsesArray(query)
    };
    this.newResponse.comment = ''; // Reset response form
  }



  loadQueryDetails(queryId: number) {
    this.studentService.getQueryById(queryId).subscribe({
      next: (query: any) => {
        console.log("Raw query details: ", query);
        
        // Transform the query to handle nested responses
        if (query.responses && query.responses.$values) {
          query.responses = query.responses.$values;
        }
        
        this.selectedQuery = query;
        console.log("Transformed query: ", this.selectedQuery);
      },
      error: (err) => {
        console.log("Error loading query details: ", err);
      }
    });
  }

  formatDate(date: Date | string): string {
    if (!date) return 'Unknown';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  deleteQuery(queryId: number) {
    if (confirm('Are you sure you want to delete this query?')) {
      this.studentService.deleteQuery(queryId).subscribe({
        next: () => {
          console.log("Query deleted successfully");
          // Reload with correct student ID
          this.studentService.getStudentIdByUserId(this.currentLoggedInUser!.userProfileID!).subscribe({
            next: (res) => {
              this.loadStudentQueries(res.studentID);
            },
            error: (err) => {
              console.log("Error getting student ID for reload: ", err);
            }
          });
        },
        error: (err) => {
          console.log("Error deleting query: ", err);
        }
      });
    }
  }


  addResponse() {
    if (!this.selectedQuery || !this.currentLoggedInUser?.userProfileID) return;
    
    this.isAddingResponse = true;
    
    this.studentService.createStudentResponse(
      this.selectedQuery.queryTopicID,
      this.currentLoggedInUser.userProfileID,
      this.newResponse
    ).subscribe({
      next: (res) => {
        console.log("Response added successfully: ", res);
        this.isAddingResponse = false;
        this.newResponse.comment = '';
        
        // Reload the query to get updated responses
        this.loadQueryDetails(this.selectedQuery!.queryTopicID);
      },
      error: (err) => {
        console.log("Error adding response: ", err);
        this.isAddingResponse = false;
      }
    });
  }


  // Helper method to get response count
  getResponseCount(query: any): number {
    if (!query || !query.responses) return 0;
    
    // Handle both direct array and nested $values structure
    if (Array.isArray(query.responses)) {
      return query.responses.length;
    } else if (query.responses.$values && Array.isArray(query.responses.$values)) {
      return query.responses.$values.length;
    }
    
    return 0;
  }

  // Helper method to get responses array
  getResponsesArray(query: any): any[] {
    if (!query || !query.responses) return [];
    
    // Handle both direct array and nested $values structure
    if (Array.isArray(query.responses)) {
      return query.responses;
    } else if (query.responses.$values && Array.isArray(query.responses.$values)) {
      return query.responses.$values;
    }
    
    return [];
  }


  // Helper method to determine if response is from tutor
  isTutorResponse(response: any): boolean {
    // Tutor responses have tutorID > 0, student responses have tutorID = -1
    return response.tutorID && response.tutorID > 0;
  }
  onLogout() {
    this.sessionUser.onLogout();
  }



   public isViewingMedia = false;
  public currentMediaUrl: string | null = null;
  public currentMediaType: string | null = null;

 

  // Add these media methods
  viewMediaFile(mediaContentUrl: string) {
    if (!mediaContentUrl) return;
    
    this.isViewingMedia = true;
    this.currentMediaUrl = `${environment.apiUrl}/topics/QueryResponses/file/${mediaContentUrl}`;
    
    // Determine file type for proper display
    const extension = mediaContentUrl.split('.').pop()?.toLowerCase();
    if (['jpg', 'jpeg', 'png', 'gif', 'webp'].includes(extension || '')) {
      this.currentMediaType = 'image';
    } else if (['pdf'].includes(extension || '')) {
      this.currentMediaType = 'pdf';
    } else if (['mp4', 'webm', 'ogg'].includes(extension || '')) {
      this.currentMediaType = 'video';
    } else if (['mp3', 'wav', 'ogg'].includes(extension || '')) {
      this.currentMediaType = 'audio';
    } else {
      this.currentMediaType = 'download';
    }
  }

  downloadFile(mediaContentUrl: string, fileName: string) {
    this.studentService.getResponseFile(mediaContentUrl).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName || 'file';
        link.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => {
        console.log('Error downloading file:', err);
        alert('Error downloading file');
      }
    });
  }

  closeMediaViewer() {
    this.isViewingMedia = false;
    this.currentMediaUrl = null;
    this.currentMediaType = null;
  }

  getMediaUrl(mediaContentUrl: string): string {
    return `${environment.apiUrl}/topics/QueryResponses/file/${mediaContentUrl}`;
  }

  isImageFile(fileName: string): boolean {
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'webp', 'bmp'];
    const extension = fileName.split('.').pop()?.toLowerCase();
    return imageExtensions.includes(extension || '');
  }

  getFileNameFromUrl(url: string): string {
    return url.split('_').pop() || 'file';
  }
}
