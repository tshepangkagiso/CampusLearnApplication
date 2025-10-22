import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { Subject } from 'rxjs';

export interface ReceiveMessageData {
  senderId: number;
  content: string;
  timestamp: string;
}

@Injectable({
  providedIn: 'root'
})
export class ChatService {
  private hubConnection: HubConnection;
  private messageReceived = new Subject<ReceiveMessageData>();

  public messageReceived$ = this.messageReceived.asObservable();

  constructor() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl('http://localhost:7000/messages/chatHub')
      .build();

    this.registerEvents();
  }

  private registerEvents() {
    this.hubConnection.on('ReceiveMessage', (senderId: number, content: string, timestamp: string) => {
      this.messageReceived.next({ senderId, content, timestamp });
    });
  }

  async startConnection(): Promise<void> {
    try {
      await this.hubConnection.start();
    } catch (err) {
      console.error('SignalR connection error:', err);
    }
  }

  joinRoom(roomId: number): Promise<void> {
    return this.hubConnection.invoke('JoinRoom', roomId);
  }

  sendMessage(roomId: number, senderId: number, content: string): Promise<void> {
    return this.hubConnection.invoke('SendMessage', roomId, senderId, content);
  }

  leaveRoom(roomId: number): Promise<void> {
    return this.hubConnection.invoke('LeaveRoom', roomId);
  }
}