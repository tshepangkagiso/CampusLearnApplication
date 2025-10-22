import { Component, inject, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AuthSessionUser } from '../../../../Interfaces/auth/auth-session-user';
import { AuthService } from '../../../../services/auth/auth-service';
import { StudentService } from '../../../../services/Student Related Services/student.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Subscription } from 'rxjs';
import { ChatRoomResponse, MessageResponse } from '../../../../models/Private Messages Models/private-messages-response.dtos';

@Component({
  selector: 'app-student-messages',
  imports: [RouterLink,FormsModule,CommonModule],
  templateUrl: './student-messages.html',
  styleUrl: './student-messages.css'
})
export class StudentMessages implements OnInit, OnDestroy {
  private sessionUser = inject(AuthService);
  private studentService = inject(StudentService);
  
  public currentLoggedInUser?: AuthSessionUser;
  public studentId: number = 0;
  public chatRooms: ChatRoomResponse[] = [];
  public selectedRoom: ChatRoomResponse | null = null;
  public messages: MessageResponse[] = [];
  public newMessage: string = '';
  public isLoading = false;
  public isConnected = false;
  public showNotification = false;
  public notificationMessage = '';
  public tutorNames: Map<number, string> = new Map(); // Cache for tutor names
  
  private messageSubscription?: Subscription;
  private notificationTimeout?: any;

  async ngOnInit() {
    const user = this.sessionUser.getUser();
    if (user?.userProfileID) {
      this.currentLoggedInUser = user;
      await this.loadStudentData(user.userProfileID);
    }
  }

  async loadStudentData(userId: number) {
    this.studentService.getStudentIdByUserId(userId).subscribe({
      next: async (res) => {
        this.studentId = res.studentID;
        await this.startChatConnection();
        this.loadStudentChatRooms(this.studentId);
      },
      error: (err) => {
        console.log("Error getting student ID: ", err);
      }
    });
  }

  async startChatConnection() {
    try {
      await this.studentService.startChatConnection();
      this.isConnected = true;
      
      // Subscribe to real-time messages
      this.messageSubscription = this.studentService.onChatMessageReceived().subscribe(
        (message) => {
          if (this.selectedRoom && message.senderId !== this.studentId) {
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
              this.showNotificationAlert('New message from tutor');
            }
          } else if (!this.selectedRoom) {
            // Show notification for new message in any room
            this.showNotificationAlert('New message from tutor');
          }
        }
      );
    } catch (error) {
      console.error('Failed to start chat connection:', error);
    }
  }

  // Helper method to extract room ID from message (you might need to adjust this based on your message structure)
  private getRoomIdFromMessage(message: any): number {
    // This depends on how your message data is structured
    // You might need to modify this based on your actual message format
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
        icon: '/assets/logo.png' // Add your app icon
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

  loadStudentChatRooms(studentId: number) {
    this.isLoading = true;
    this.studentService.getStudentChatRooms(studentId).subscribe({
      next: (res: any) => {
        // Handle API response structure
        if (Array.isArray(res)) {
          this.chatRooms = res;
        } else if (res && Array.isArray(res.chatRooms)) {
          this.chatRooms = res.chatRooms;
        } else {
          this.chatRooms = [];
        }
        
        // Pre-load tutor names for all chat rooms
        this.loadTutorNames();
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading chat rooms: ", err);
        this.chatRooms = [];
        this.isLoading = false;
      }
    });
  }

  // Load tutor names using existing service methods
  loadTutorNames() {
    this.chatRooms.forEach(room => {
      if (room.tutorId && !this.tutorNames.has(room.tutorId)) {
        // Use the existing service to get tutor info
        // Since we don't have a direct method, we'll use a placeholder
        // You can replace this with actual tutor name lookup if available
        this.tutorNames.set(room.tutorId, `Tutor`);
      }
    });
  }

  getTutorDisplayName(tutorId: number): string {
    return this.tutorNames.get(tutorId) || 'Tutor';
  }

  async selectChatRoom(room: ChatRoomResponse) {
    this.selectedRoom = room;
    await this.studentService.joinChatRoom(room.roomId);
    this.loadChatRoomMessages(room.roomId);
    
    // Hide any active notifications when selecting a room
    this.closeNotification();
  }

  loadChatRoomMessages(roomId: number) {
    this.studentService.getChatRoomMessages(roomId).subscribe({
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
      await this.studentService.sendChatMessage(
        this.selectedRoom.roomId,
        this.studentId,
        this.newMessage
      );

      // Add message to local list immediately
      this.messages.push({
        messageId: 0, // Temporary ID
        roomId: this.selectedRoom.roomId,
        senderId: this.studentId,
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
    return senderId === this.studentId;
  }

  getSenderDisplayName(senderId: number): string {
    if (senderId === this.studentId) {
      return 'You';
    } else {
      return this.getTutorDisplayName(senderId);
    }
  }

  ngOnDestroy() {
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
    if (this.selectedRoom) {
      this.studentService.leaveChatRoom(this.selectedRoom.roomId);
    }
    if (this.notificationTimeout) {
      clearTimeout(this.notificationTimeout);
    }
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}
