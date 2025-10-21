export interface ChatRequest {
  studentId: number;
  question: string;
  moduleCode: string;
}

export interface AIAgentRequest {
  studentId: number;
  question: string;
  moduleCode: string;
}


export interface ChatResponse {
  response: string;
}

export interface AIAgentResponse {
    output: string
}