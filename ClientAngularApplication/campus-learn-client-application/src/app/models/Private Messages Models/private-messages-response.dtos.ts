
export interface SendMessageRequest {
  roomId: number;
  senderId: number;
  content: string;
}

export interface ChatRoomResponse {
  roomId: number;
  studentId: number;
  tutorId: number;
  queryId: number;
  title: string;
  moduleCode: string;
  createdAt: Date;
  isActive: boolean;
  messages: MessageResponse[];
}

export interface MessageResponse {
  messageId: number;
  roomId: number;
  senderId: number;
  content: string;
  timestamp: Date;
  mediaContentUrl?: string;
}

export interface ChatRoomsListResponse {
  chatRooms: ChatRoomResponse[];
}

export interface MessagesListResponse {
  messages: MessageResponse[];
}

export interface ChatRoomStatsResponse {
  totalChatRooms: number;
  activeChatRooms: number;
  inactiveChatRooms: number;
  totalMessages: number;
  averageMessagesPerRoom: number;
}