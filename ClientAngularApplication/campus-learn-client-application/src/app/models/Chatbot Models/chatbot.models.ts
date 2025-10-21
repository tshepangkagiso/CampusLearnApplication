export interface ChatbotResponse {
  response: string;
  studentId: number;
  moduleCode: string;
}

export interface AIAgentResponse {
  response: string;
  suggestions?: string[];
  escalated?: boolean;
  confidence?: number;
}