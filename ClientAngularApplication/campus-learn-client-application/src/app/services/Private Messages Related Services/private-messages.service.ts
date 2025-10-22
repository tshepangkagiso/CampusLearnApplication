import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map, Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { ChatRoomsListResponse, ChatRoomResponse, ChatRoomStatsResponse, MessagesListResponse, MessageResponse } from '../../models/Private Messages Models/private-messages-response.dtos';


@Injectable({
  providedIn: 'root'
})
export class PrivateMessagesService {
  private baseUrl = `${environment.apiUrl}/messages/message`;

  constructor(private http: HttpClient) {}

  // Helper method to extract $values from API response
  private extractValues<T>(response: any): T[] {
    if (response && response.$values && Array.isArray(response.$values)) {
      return response.$values;
    } else if (Array.isArray(response)) {
      return response;
    }
    return [];
  }

  // Helper method to extract nested $values from messages
  private extractMessages(response: any): MessageResponse[] {
    if (response && response.messages && response.messages.$values && Array.isArray(response.messages.$values)) {
      return response.messages.$values;
    } else if (response && Array.isArray(response.messages)) {
      return response.messages;
    } else if (Array.isArray(response)) {
      return response;
    } else if (response && response.$values && Array.isArray(response.$values)) {
      return response.$values;
    }
    return [];
  }

  // Get all chat rooms
  getAllChatRooms(): Observable<ChatRoomsListResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms`).pipe(
      map(response => ({
        chatRooms: this.extractValues<ChatRoomResponse>(response)
      }))
    );
  }

  // Get chat room by ID
  getChatRoomById(roomId: number): Observable<ChatRoomResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms/${roomId}`).pipe(
      map(response => {
        // Handle nested messages structure
        if (response && response.messages) {
          response.messages = this.extractMessages(response);
        }
        return response;
      })
    );
  }

  // Get chat rooms for student
  getStudentChatRooms(studentId: number): Observable<ChatRoomsListResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms/student/${studentId}`).pipe(
      map(response => ({
        chatRooms: this.extractValues<ChatRoomResponse>(response).map(room => ({
          ...room,
          messages: this.extractMessages(room.messages)
        }))
      }))
    );
  }

  // Get chat rooms for tutor
  getTutorChatRooms(tutorId: number): Observable<ChatRoomsListResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms/tutor/${tutorId}`).pipe(
      map(response => ({
        chatRooms: this.extractValues<ChatRoomResponse>(response).map(room => ({
          ...room,
          messages: this.extractMessages(room.messages)
        }))
      }))
    );
  }

  // Get specific chat room
  getChatRoomByStudentTutorQuery(studentId: number, tutorId: number, queryId: number): Observable<ChatRoomResponse> {
    return this.http.get<any>(
      `${this.baseUrl}/chatrooms/student/${studentId}/tutor/${tutorId}/query/${queryId}`
    ).pipe(
      map(response => {
        if (response && response.messages) {
          response.messages = this.extractMessages(response);
        }
        return response;
      })
    );
  }

  // Get active chat rooms
  getActiveChatRooms(): Observable<ChatRoomsListResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms/active`).pipe(
      map(response => ({
        chatRooms: this.extractValues<ChatRoomResponse>(response).map(room => ({
          ...room,
          messages: this.extractMessages(room.messages)
        }))
      }))
    );
  }

  // Get chat room stats
  getChatRoomStats(): Observable<ChatRoomStatsResponse> {
    return this.http.get<ChatRoomStatsResponse>(`${this.baseUrl}/chatrooms/stats`);
  }

  // Get messages for chat room
  getChatRoomMessages(roomId: number): Observable<MessagesListResponse> {
    return this.http.get<any>(`${this.baseUrl}/chatrooms/${roomId}/messages`).pipe(
      map(response => ({
        messages: this.extractValues<MessageResponse>(response)
      }))
    );
  }

  // Get queued messages
  getQueuedMessages(): Observable<MessageResponse[]> {
    return this.http.get<any>(`${this.baseUrl}`).pipe(
      map(response => this.extractValues<MessageResponse>(response))
    );
  }

  // Get next queued message
  getNextQueuedMessage(): Observable<MessageResponse> {
    return this.http.get<MessageResponse>(`${this.baseUrl}/next`);
  }
}