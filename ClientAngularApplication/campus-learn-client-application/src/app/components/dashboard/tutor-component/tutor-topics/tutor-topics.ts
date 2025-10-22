import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { CreateTutorResponseRequest, UpdateQueryRequest } from '../../../../models/Topics Related Models/topics-request.dtos';
import { TutorQueriesResponse, QueryTopicResponse } from '../../../../models/Topics Related Models/topics-response.dtos';
import { environment } from '../../../../models/environments/environment';

@Component({
  selector: 'app-tutor-topics',
  imports: [RouterLink, FormsModule, CommonModule],
  templateUrl: './tutor-topics.html',
  styleUrl: './tutor-topics.css'
})
export class TutorTopics {
  private sessionUser = inject(AuthService);
  private tutorService = inject(TutorService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public tutorQueries: TutorQueriesResponse = {
    tutorId: 0,
    totalQueries: 0,
    queries: []
  };

    public selectedFile: File | null = null;

  // Media viewing state
  public isViewingMedia = false;
  public currentMediaUrl: string | null = null;
  public currentMediaType: string | null = null;

    onFileSelected(event: any) {
      const file = event.target.files[0];
      if (file) {
        this.selectedFile = file;
        console.log('File selected:', file.name);
      }
    }

  
  // UI State
  public isLoading = false;
  public selectedQuery: QueryTopicResponse | null = null;
  public isAddingResponse = false;
  
  // Form Models
  public newResponse: CreateTutorResponseRequest = {
    comment: '',
    isSolution: false
  };

  ngOnInit(): void {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      
      this.tutorService.getTutorIdByUserId(user.userProfileID).subscribe({
        next: (res) => { 
          console.log('Tutor ID:', res.tutorID);
          const tutorId = res.tutorID;
          this.loadTutorQueries(tutorId);
        },
        error: (err) => {
          console.log("Error getting tutor ID: ", err);
        }
      });
    }
  }

  loadTutorQueries(tutorId: number) {
    this.isLoading = true;

    this.tutorService.getTutorQueries(tutorId).subscribe({
      next: (res: any) => {
        console.log("Raw tutor queries response: ", res);
        
        // Transform the response to handle nested structure
        this.tutorQueries = {
          tutorId: res.tutorId,
          totalQueries: res.totalQueries,
          queries: res.queries?.$values || []  // Extract the nested array
        };
        
        console.log("Transformed tutor queries: ", this.tutorQueries);
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading tutor queries: ", err);
        this.isLoading = false;
      }
    });
  }

  viewQueryDetails(query: any) {
    this.selectedQuery = {
      ...query,
      responses: this.getResponsesArray(query)
    };
    this.newResponse.comment = '';
    this.newResponse.isSolution = false;
  }

  addResponse() {
    if (!this.selectedQuery || !this.currentLoggedInUser?.userProfileID) return;
    
    this.isAddingResponse = true;
    
    this.tutorService.getTutorIdByUserId(this.currentLoggedInUser.userProfileID).subscribe({
      next: (tutorRes) => {
        const tutorId = tutorRes.tutorID;
        
        const request: CreateTutorResponseRequest = {
          comment: this.newResponse.comment,
          mediaContent: this.selectedFile || undefined,
          isSolution: this.newResponse.isSolution
        };
        
        this.tutorService.createTutorResponse(
          this.selectedQuery!.queryTopicID,
          tutorId,
          request
        ).subscribe({
          next: (res) => {
            console.log("Tutor response added successfully: ", res);
            this.isAddingResponse = false;
            this.newResponse.comment = '';
            this.newResponse.isSolution = false;
            this.selectedFile = null;
            
            // Reset file input
            const fileInput = document.getElementById('mediaFile') as HTMLInputElement;
            if (fileInput) fileInput.value = '';
            
            this.loadTutorQueries(tutorId);
          },
          error: (err) => {
            console.log("Error adding tutor response: ", err);
            this.isAddingResponse = false;
          }
        });
      },
      error: (err) => {
        console.log("Error getting tutor ID: ", err);
        this.isAddingResponse = false;
      }
    });
  }

  removeSelectedFile() {
    this.selectedFile = null;
    const fileInput = document.getElementById('mediaFile') as HTMLInputElement;
    if (fileInput) fileInput.value = '';
  }


  // Helper methods
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

  getResponseCount(query: any): number {
    if (!query || !query.responses) return 0;
    if (Array.isArray(query.responses)) return query.responses.length;
    if (query.responses.$values && Array.isArray(query.responses.$values)) return query.responses.$values.length;
    return 0;
  }

  getResponsesArray(query: any): any[] {
    if (!query || !query.responses) return [];
    if (Array.isArray(query.responses)) return query.responses;
    if (query.responses.$values && Array.isArray(query.responses.$values)) return query.responses.$values;
    return [];
  }

  isTutorResponse(response: any): boolean {
    return response.tutorID && response.tutorID > 0;
  }

  onLogout() {
    this.sessionUser.onLogout();
  }


    // Method to view media file
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

  // Method to download file
  downloadFile(mediaContentUrl: string, fileName: string) {
    this.tutorService.getResponseFile(mediaContentUrl).subscribe({
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

  // Helper to get file name from URL
  getFileNameFromUrl(url: string): string {
    return url.split('_').pop() || 'file';
  }


// Add these helper methods to your component
getMediaUrl(mediaContentUrl: string): string {
  return `${environment.apiUrl}/topics/QueryResponses/file/${mediaContentUrl}`;
}

isImageFile(fileName: string): boolean {
  const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'webp', 'bmp'];
  const extension = fileName.split('.').pop()?.toLowerCase();
  return imageExtensions.includes(extension || '');
}

isPdfFile(fileName: string): boolean {
  return fileName.toLowerCase().endsWith('.pdf');
}

isVideoFile(fileName: string): boolean {
  const videoExtensions = ['mp4', 'webm', 'ogg', 'mov', 'avi'];
  const extension = fileName.split('.').pop()?.toLowerCase();
  return videoExtensions.includes(extension || '');
}

isAudioFile(fileName: string): boolean {
  const audioExtensions = ['mp3', 'wav', 'ogg', 'm4a'];
  const extension = fileName.split('.').pop()?.toLowerCase();
  return audioExtensions.includes(extension || '');
}
}
