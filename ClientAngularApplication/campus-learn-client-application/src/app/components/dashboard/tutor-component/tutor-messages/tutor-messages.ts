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
  
  private messageSubscription?: Subscription;

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
            this.messages.push({
              messageId: 0, // Temporary ID
              roomId: this.selectedRoom.roomId,
              senderId: message.senderId,
              content: message.content,
              timestamp: new Date(message.timestamp)
            });
          }
        }
      );
    } catch (error) {
      console.error('Failed to start chat connection:', error);
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
        this.isLoading = false;
      },
      error: (err) => {
        console.log("Error loading chat rooms: ", err);
        this.chatRooms = [];
        this.isLoading = false;
      }
    });
  }

  async selectChatRoom(room: ChatRoomResponse) {
    this.selectedRoom = room;
    await this.tutorService.joinChatRoom(room.roomId);
    this.loadChatRoomMessages(room.roomId);
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

  ngOnDestroy() {
    if (this.messageSubscription) {
      this.messageSubscription.unsubscribe();
    }
    if (this.selectedRoom) {
      this.tutorService.leaveChatRoom(this.selectedRoom.roomId);
    }
  }

  onLogout() {
    this.sessionUser.onLogout();
  }
}