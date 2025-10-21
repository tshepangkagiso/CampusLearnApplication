
export interface ChatRoom {
  roomId: number;
  studentId: number;
  tutorId: number;
  queryId: number;
  title: string;
  moduleCode: string;
  createdAt: Date;
  isActive: boolean;
  messages?: Message[];
}

export interface Message {
  messageId: number;
  roomId: number;
  senderId: number;
  content: string;
  timestamp: Date;
  mediaContentUrl?: string;
}

