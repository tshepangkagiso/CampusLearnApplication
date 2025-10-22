export interface AIAgentRequest {
  studentId: number;
  question: string;
  moduleCode: string;
}

export interface AIAgentResponse {
    output: string
}