import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { StudentService } from '../../../../services/Student Related Services/student.service';
import { AIAgentRequest, AIAgentResponse } from '../../../../models/Chatbot Models/chatbot.dtos';
import { ChatbotService } from '../../../../services/Chatbot Related Services/chatbot.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-student-chatbot',
  imports: [RouterLink,FormsModule, CommonModule],
  templateUrl: './student-chatbot.html',
  styleUrl: './student-chatbot.css'
})
export class StudentChatbot {
 private sessionUser = inject(AuthService);
  private studentService = inject(StudentService);
  private chatbotService = inject(ChatbotService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public student: any;
  public subscribedModules: any[] = [];

  // Chat state
  public messages: Array<{
    type: 'user' | 'bot';
    content: string;
    timestamp: Date;
  }> = [];

  public userInput = '';
  public selectedModule = '';
  public isLoading = false;

  ngOnInit(): void {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      this.loadStudentData(user.userProfileID);
    }
  }

  loadStudentData(studentId: number) {
    this.studentService.getStudentProfile(studentId).subscribe({
      next: (res) => {
        this.student = res;
      }
    });

    this.studentService.getSubscribedModules(studentId).subscribe({
      next: (res) => {
        this.subscribedModules = res.subscribedModules || [];
      }
    });
  }

  sendMessage() {
    if (!this.userInput.trim() || !this.selectedModule) return;

    const userMessage = this.userInput.trim();
    this.addMessage('user', userMessage);
    
    this.isLoading = true;

    const request: AIAgentRequest = {
      studentId: this.currentLoggedInUser!.userProfileID,
      question: userMessage,
      moduleCode: this.selectedModule
    };

    this.chatbotService.chatWithAIAgent(request).subscribe({
      next: (response: AIAgentResponse) => {
        this.addMessage('bot', response.output); 
        this.isLoading = false;
        this.userInput = '';
      },
      error: (err) => {
        this.addMessage('bot', 'Sorry, I encountered an error. Please try again.');
        this.isLoading = false;
        console.error('Chatbot error:', err);
      }
    });
  }

  addMessage(type: 'user' | 'bot', content: string) {
    this.messages.push({
      type,
      content,
      timestamp: new Date()
    });
  }

  formatTime(date: Date): string {
    return date.toLocaleTimeString('en-US', { 
      hour: '2-digit', 
      minute: '2-digit' 
    });
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}
