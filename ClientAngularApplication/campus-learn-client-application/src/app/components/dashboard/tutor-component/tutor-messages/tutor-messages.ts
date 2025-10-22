import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { TutorService } from '../../../../services/Tutor Related Services/tutor.service';
import { Subscription } from 'rxjs';
import { ChatRoomResponse, MessageResponse } from '../../../../models/Private Messages Models/private-messages-response.dtos';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-tutor-messages',
  imports: [RouterLink,FormsModule,CommonModule],
  templateUrl: './tutor-messages.html',
  styleUrl: './tutor-messages.css'
})
export class TutorMessages implements OnInit, OnDestroy {
  private sessionUser = inject(AuthService);
  private tutorService = inject(TutorService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public tutorId: number = 0;
  public chatRooms: ChatRoomResponse[] = [];
  public selectedRoom: ChatRoomResponse | null = null;
  public messages: MessageResponse[] = [];
  public newMessage: string = '';
  public isLoading = false;
  public isConnected = false;
  public showNotification = false;
  public notificationMessage = '';
  public studentNames: Map<number, string> = new Map(); // Cache for student names
  
  private messageSubscription?: Subscription;
  private notificationTimeout?: any;

  async ngOnInit() {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      await this.loadTutorData(user.userProfileID);
    }
  }

  async loadTutorData(userId: number) {
    this.tutorService.getTutorIdByUserId(userId).subscribe({
      next: async (res) => {
        this.tutorId = res.tutorID;
        await this.startChatConnection();
        this.loadTutorChatRooms(this.tutorId);
      },
      error: (err) => {
        console.log("Error getting tutor ID: ", err);
      }
    });
  }

  async startChatConnection() {
    try {
      await this.tutorService.startChatConnection();
      this.isConnected = true;
      
      // Subscribe to real-time messages
      this.messageSubscription = this.tutorService.onChatMessageReceived().subscribe(
        (message) => {
          if (this.selectedRoom && message.senderId !== this.tutorId) {
            // Add message to list
            this.messages.push({
              messageId: 0, // Temporary ID
              roomId: this.selectedRoom.roomId,
              senderId: message.senderId,
              content: message.content,
              timestamp: new Date(message.timestamp)
            });

            // Show notification if not in this chat room or if page is not focused
            if (this.selectedRoom.roomId !== this.getRoomIdFromMessage(message) || !document.hasFocus()) {
              this.showNotificationAlert('New message from student');
            }
          } else if (!this.selectedRoom) {
            // Show notification for new message in any room
            this.showNotificationAlert('New message from student');
          }
        }
      );
    } catch (error) {
      console.error('Failed to start chat connection:', error);
    }
  }

  private getRoomIdFromMessage(message: any): number {
    return message.roomId || 0;
  }

  showNotificationAlert(message: string) {
    this.notificationMessage = message;
    this.showNotification = true;

    // Auto-hide notification after 5 seconds
    if (this.notificationTimeout) {
      clearTimeout(this.notificationTimeout);
    }
    
    this.notificationTimeout = setTimeout(() => {
      this.showNotification = false;
    }, 5000);

    // Browser notification (if permitted)
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification('CampusLearn Message', {
        body: message,
        icon: '/assets/logo.png'
      });
    }
  }

  closeNotification() {
    this.showNotification = false;
    if (this.notificationTimeout) {
      clearTimeout(this.notificationTimeout);
    }
  }

  // Request browser notification permission
  requestNotificationPermission() {
    if ('Notification' in window && Notification.permission === 'default') {
      Notification.requestPermission();
    }
  }

  loadTutorChatRooms(tutorId: number) {
    this.isLoading = true;
    this.tutorService.getTutorChatRooms(tutorId).subscribe({
      next: (res: any) => {
        // Handle API response structure
        if (Array.isArray(res)) {
          this.chatRooms = res;
        } else if (res && Array.isArray(res.chatRooms)) {
          this.chatRooms = res.chatRooms;
        } else {
          this.chatRooms = [];
        }
        
        // Load student names for all chat rooms
        this.loadStudentNames();
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading chat rooms: ", err);
        this.chatRooms = [];
        this.isLoading = false;
      }
    });
  }

  // Load student names using existing services
  loadStudentNames() {
    this.chatRooms.forEach(room => {
      if (room.studentId && !this.studentNames.has(room.studentId)) {
        this.getStudentName(room.studentId);
      }
    });
  }

  getStudentName(studentId: number) {
    this.studentNames.set(studentId, 'Student');
  }

  getStudentDisplayName(studentId: number): string {
    return this.studentNames.get(studentId) || 'Student';
  }

  async selectChatRoom(room: ChatRoomResponse) {
    this.selectedRoom = room;
    await this.tutorService.joinChatRoom(room.roomId);
    this.loadChatRoomMessages(room.roomId);
    
    // Hide any active notifications when selecting a room
    this.closeNotification();
  }

  loadChatRoomMessages(roomId: number) {
    this.tutorService.getChatRoomMessages(roomId).subscribe({
      next: (res: any) => {
        if (Array.isArray(res)) {
          this.messages = res;
        } else if (res && Array.isArray(res.messages)) {
          this.messages = res.messages;
        } else {
          this.messages = [];
        }
      },
      error: (err) => {
        console.log("Error loading messages: ", err);
        this.messages = [];
      }
    });
  }

  async sendMessage() {
    if (!this.newMessage.trim() || !this.selectedRoom) return;

    try {
      await this.tutorService.sendChatMessage(
        this.selectedRoom.roomId,
        this.tutorId,
        this.newMessage
      );

      // Add message to local list immediately
      this.messages.push({
        messageId: 0, // Temporary ID
        roomId: this.selectedRoom.roomId,
        senderId: this.tutorId,
        content: this.newMessage,
        timestamp: new Date()
      });

      this.newMessage = '';
    } catch (error) {
      console.error('Failed to send message:', error);
    }
  }

  formatDate(date: Date | string): string {
    if (!date) return '';
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toLocaleTimeString('en-US', {
      hour: '2-digit',
      minute: '2-digit'
    });
  }

  isMyMessage(senderId: number): boolean {
    return senderId === this.tutorId;
  }

  getSenderDisplayName(senderId: number): string {
    if (senderId === this.tutorId) {
      return 'You';
    } else {
      return this.getStudentDisplayName(senderId);
    }
  }

  getMessageCount(room: ChatRoomResponse): number {
    return room.messages?.length || 0;
  }

  ngOnDestroy() {
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
    if (this.selectedRoom) {
      this.tutorService.leaveChatRoom(this.selectedRoom.roomId);
    }
    if (this.notificationTimeout) {
      clearTimeout(this.notificationTimeout);
    }
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}