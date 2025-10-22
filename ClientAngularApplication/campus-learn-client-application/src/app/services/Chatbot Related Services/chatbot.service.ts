import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AIAgentRequest } from '../../models/Chatbot Models/chatbot.dtos';
import { AIAgentResponse } from '../../models/Chatbot Models/chatbot.models';
import { environment } from '../../models/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private baseUrl = `${environment.apiUrl}/agent/chatbot`;

  constructor(private http: HttpClient) {}

  // n8n AI agent with Gemini
  chatWithAIAgent(request: AIAgentRequest): Observable<AIAgentResponse> {
    return this.http.post<AIAgentResponse>(`${this.baseUrl}/n8n/ai-agent`, request);
  }
}