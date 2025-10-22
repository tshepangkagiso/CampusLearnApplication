import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { ChatRoomsListResponse, ChatRoomResponse, ChatRoomStatsResponse, MessagesListResponse, MessageResponse } from '../../models/Private Messages Models/private-messages-response.dtos';


@Injectable({
  providedIn: 'root'
})
export class PrivateMessagesService {
  private baseUrl = `${environment.apiUrl}/messages/message`;

  constructor(private http: HttpClient) {}

  // Get all chat rooms
  getAllChatRooms(): Observable<ChatRoomsListResponse> {
    return this.http.get<ChatRoomsListResponse>(`${this.baseUrl}/chatrooms`);
  }

  // Get chat room by ID
  getChatRoomById(roomId: number): Observable<ChatRoomResponse> {
    return this.http.get<ChatRoomResponse>(`${this.baseUrl}/chatrooms/${roomId}`);
  }

  // Get chat rooms for student
  getStudentChatRooms(studentId: number): Observable<ChatRoomsListResponse> {
    return this.http.get<ChatRoomsListResponse>(`${this.baseUrl}/chatrooms/student/${studentId}`);
  }

  // Get chat rooms for tutor
  getTutorChatRooms(tutorId: number): Observable<ChatRoomsListResponse> {
    return this.http.get<ChatRoomsListResponse>(`${this.baseUrl}/chatrooms/tutor/${tutorId}`);
  }

  // Get specific chat room
  getChatRoomByStudentTutorQuery(studentId: number, tutorId: number, queryId: number): Observable<ChatRoomResponse> {
    return this.http.get<ChatRoomResponse>(
      `${this.baseUrl}/chatrooms/student/${studentId}/tutor/${tutorId}/query/${queryId}`
    );
  }

  // Get active chat rooms
  getActiveChatRooms(): Observable<ChatRoomsListResponse> {
    return this.http.get<ChatRoomsListResponse>(`${this.baseUrl}/chatrooms/active`);
  }

  // Get chat room stats
  getChatRoomStats(): Observable<ChatRoomStatsResponse> {
    return this.http.get<ChatRoomStatsResponse>(`${this.baseUrl}/chatrooms/stats`);
  }

  // Get messages for chat room
  getChatRoomMessages(roomId: number): Observable<MessagesListResponse> {
    return this.http.get<MessagesListResponse>(`${this.baseUrl}/chatrooms/${roomId}/messages`);
  }

  // Get queued messages
  getQueuedMessages(): Observable<MessageResponse[]> {
    return this.http.get<MessageResponse[]>(`${this.baseUrl}`);
  }

  // Get next queued message
  getNextQueuedMessage(): Observable<MessageResponse> {
    return this.http.get<MessageResponse>(`${this.baseUrl}/next`);
  }
}