import { Component, inject, OnInit } from "@angular/core";
import { RouterLink } from "@angular/router";
import { AuthSessionUser } from "../../../../Interfaces/auth/auth-session-user";
import { CreateForumTopicRequest, CreateForumResponseRequest } from "../../../../models/Forum Related Models/forum-request.dtos";
import { ForumTopicResponse, ForumTopicListResponse, ForumResponsesResponse } from "../../../../models/Forum Related Models/forum-response.dtos";
import { ForumTopic } from "../../../../models/Forum Related Models/forum.models";
import { AuthService } from "../../../../services/auth/auth-service";
import { StudentService } from "../../../../services/Student Related Services/student.service";
import { FormsModule } from "@angular/forms";
import { CommonModule, DatePipe } from "@angular/common";
import { DomSanitizer, SafeUrl } from "@angular/platform-browser";


@Component({
  selector: 'app-student-forums',
  imports: [RouterLink,DatePipe,FormsModule,CommonModule],
  templateUrl: './student-forums.html',
  styleUrl: './student-forums.css'
})
export class StudentForums implements OnInit {

  private sessionUser = inject(AuthService);
  private studentService = inject(StudentService);
  private sanitizer = inject(DomSanitizer);

  public currentLoggedInUser!: AuthSessionUser;
  
  // Forum data
  public forumTopics: any[] = [];
  public selectedTopic: any;
  public topicResponses: any[] = [];
  
  // Subscribed modules for dropdown
  public subscribedModules: any[] = [];
  
  // New topic form
  public newTopic: CreateForumTopicRequest = {
    title: '',
    description: '',
    moduleCode: '',
    userProfileID: 0,
    isAnonymous: false,
    anonymousName: ''
  };
  
  // New response form
  public newResponse: CreateForumResponseRequest = {
    comment: '',
    userProfileID: 0,
    isAnonymous: false,
    anonymousName: ''
  };
  
  // UI state
  public loading = false;
  public showTopicForm = false;
  public selectedFile?: File;
  public errorMessage = '';
  public successMessage = '';

  // Base URL for media files
  private mediaBaseUrl = 'http://localhost:7000/forums/ForumTopicResponse/file';

  ngOnInit(): void {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      this.newTopic.userProfileID = user.userProfileID;
      this.newResponse.userProfileID = user.userProfileID;
      this.loadSubscribedModules();
      this.loadForumTopics();
    } else {
      console.log("Empty user profile ID: " + user?.userProfileID);
    }
  }

  // Get full media URL
  getMediaUrl(fileName: string): SafeUrl {
    const fullUrl = `${this.mediaBaseUrl}/${fileName}`;
    return this.sanitizer.bypassSecurityTrustUrl(fullUrl);
  }

  // Check file type for dynamic display
  getFileType(fileName: string): string {
    if (!fileName) return 'file';
    
    const extension = fileName.toLowerCase().split('.').pop();
    const imageExtensions = ['jpg', 'jpeg', 'png', 'gif', 'bmp', 'webp'];
    const videoExtensions = ['mp4', 'avi', 'mov', 'wmv', 'flv', 'webm'];
    const audioExtensions = ['mp3', 'wav', 'ogg', 'aac', 'flac'];
    
    if (imageExtensions.includes(extension || '')) return 'image';
    if (videoExtensions.includes(extension || '')) return 'video';
    if (audioExtensions.includes(extension || '')) return 'audio';
    return 'file';
  }

  // Get display name for response author
  getResponseAuthor(response: any): string {
    if (response.isAnonymous) {
      return response.anonymousName || 'Anonymous';
    }
    
    // Check if it's the current user
    if (response.userProfileID === this.currentLoggedInUser?.userProfileID) {
      return `${this.currentLoggedInUser.name} ${this.currentLoggedInUser.surname}`;
    }
    
    // Check if user object exists and has name/surname
    if (response.user && response.user.name && response.user.surname) {
      return `${response.user.name} ${response.user.surname}`;
    }
    
    // Fallback to user ID if no name available
    return `User ${response.userProfileID}`;
  }

  // Get display name for topic author
  getTopicAuthor(topic: any): string {
    if (topic.isAnonymous) {
      return topic.anonymousName || 'Anonymous';
    }
    
    // Check if it's the current user
    if (topic.userProfileID === this.currentLoggedInUser?.userProfileID) {
      return `${this.currentLoggedInUser.name} ${this.currentLoggedInUser.surname}`;
    }
    
    // Check if user object exists and has name/surname
    if (topic.user && topic.user.name && topic.user.surname) {
      return `${topic.user.name} ${topic.user.surname}`;
    }
    
    // Fallback to user ID if no name available
    return `User ${topic.userProfileID}`;
  }

  // Load subscribed modules for dropdown
  loadSubscribedModules(): void {
    if (!this.currentLoggedInUser?.userProfileID) return;
    
    this.studentService.getSubscribedModules(this.currentLoggedInUser.userProfileID).subscribe({
      next: (response: any) => {
        this.subscribedModules = response.subscribedModules || [];
      },
      error: (error) => {
        console.error('Error loading subscribed modules:', error);
        this.errorMessage = 'Failed to load your subscribed modules';
      }
    });
  }

  // Load all forum topics using StudentService
  loadForumTopics(): void {
    this.loading = true;
    this.studentService.getAllForumTopics().subscribe({
      next: (response: any) => {
        this.forumTopics = response.topics || response || [];
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading forum topics:', error);
        this.errorMessage = 'Failed to load forum topics';
        this.loading = false;
      }
    });
  }

  // Select a topic to view its responses
  selectTopic(topic: any): void {
    this.selectedTopic = topic;
    this.loadTopicResponses(topic.forumTopicID);
  }

  // Load responses for a specific topic using StudentService
  loadTopicResponses(topicId: number): void {
    this.loading = true;
    this.studentService.getForumTopicResponses(topicId).subscribe({
      next: (response: any) => {
        this.topicResponses = response.responses || response || [];
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading topic responses:', error);
        this.errorMessage = 'Failed to load topic responses';
        this.loading = false;
      }
    });
  }

  // Create a new forum topic using StudentService
  createTopic(): void {
    if (!this.newTopic.title || !this.newTopic.description || !this.newTopic.moduleCode) {
      this.errorMessage = 'Please fill in all required fields';
      return;
    }

    this.loading = true;
    this.studentService.createForumTopic(this.newTopic).subscribe({
      next: (response: any) => {
        this.successMessage = 'Topic created successfully!';
        this.showTopicForm = false;
        this.resetTopicForm();
        this.loadForumTopics();
        this.loading = false;
      },
      error: (error) => {
        console.error('Error creating topic:', error);
        this.errorMessage = 'Failed to create topic';
        this.loading = false;
      }
    });
  }

  // Create a new response to a topic using StudentService
  createResponse(): void {
    if (!this.selectedTopic || !this.newResponse.comment) {
      this.errorMessage = 'Please write a response';
      return;
    }

    // Add the selected file to the request
    if (this.selectedFile) {
      this.newResponse.mediaContent = this.selectedFile;
    }

    this.loading = true;
    this.studentService.createForumResponse(this.selectedTopic.forumTopicID, this.newResponse).subscribe({
      next: (response: any) => {
        this.successMessage = 'Response posted successfully!';
        this.resetResponseForm();
        this.loadTopicResponses(this.selectedTopic.forumTopicID);
        this.loading = false;
      },
      error: (error) => {
        console.error('Error creating response:', error);
        this.errorMessage = 'Failed to post response';
        this.loading = false;
      }
    });
  }

  // Handle file selection for media content
  onFileSelected(event: any): void {
    const file = event.target.files[0];
    if (file) {
      this.selectedFile = file;
    }
  }

  // Upvote a topic using StudentService
  upvoteTopic(topicId: number): void {
    this.studentService.upvoteForumTopic(topicId).subscribe({
      next: (response: any) => {
        const topic = this.forumTopics.find((t: any) => t.forumTopicID === topicId);
        if (topic) {
          topic.topicUpVote = response.upvotes;
        }
      },
      error: (error) => {
        console.error('Error upvoting topic:', error);
        this.errorMessage = 'Failed to upvote topic';
      }
    });
  }

  // Upvote a response using StudentService
  upvoteResponse(responseId: number): void {
    this.studentService.upvoteForumResponse(responseId).subscribe({
      next: (response: any) => {
        const forumResponse = this.topicResponses.find((r: any) => r.responseID === responseId);
        if (forumResponse) {
          forumResponse.responseUpVote = response.upvotes;
        }
      },
      error: (error) => {
        console.error('Error upvoting response:', error);
        this.errorMessage = 'Failed to upvote response';
      }
    });
  }

  // Reset forms
  resetTopicForm(): void {
    this.newTopic = {
      title: '',
      description: '',
      moduleCode: '',
      userProfileID: this.currentLoggedInUser?.userProfileID || 0,
      isAnonymous: false,
      anonymousName: ''
    };
  }

  resetResponseForm(): void {
    this.newResponse = {
      comment: '',
      userProfileID: this.currentLoggedInUser?.userProfileID || 0,
      isAnonymous: false,
      anonymousName: ''
    };
    this.selectedFile = undefined;
  }

  // Clear messages
  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  // Download a file using StudentService
  downloadFile(fileName: string): void {
    this.studentService.getForumFile(fileName).subscribe({
      next: (blob: Blob) => {
        // Create a download link
        const url = window.URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        
        const parts = fileName.split('_');
        const originalFileName = parts.length > 1 ? parts.slice(1).join('_') : fileName;
        
        link.download = originalFileName;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      },
      error: (error) => {
        console.error('Error downloading file:', error);
        this.errorMessage = 'Failed to download file';
      }
    });
  }

  onLogout(): void {
    this.sessionUser.onLogout();
  }
}