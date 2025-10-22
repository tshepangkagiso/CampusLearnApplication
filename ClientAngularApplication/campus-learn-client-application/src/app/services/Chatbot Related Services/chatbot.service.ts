import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ChatRequest, ChatResponse, AIAgentRequest } from '../../models/Chatbot Models/chatbot.dtos';
import { AIAgentResponse } from '../../models/Chatbot Models/chatbot.models';
import { environment } from '../../models/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private baseUrl = `${environment.apiUrl}/agent/chatbot`;

  constructor(private http: HttpClient) {}

  // DeepSeek chatbot
  chatWithBot(request: ChatRequest): Observable<ChatResponse> {
    return this.http.post<ChatResponse>(`${this.baseUrl}/chatbot`, request);
  }

  // n8n AI agent with Gemini
  chatWithAIAgent(request: AIAgentRequest): Observable<AIAgentResponse> {
    return this.http.post<AIAgentResponse>(`${this.baseUrl}/n8n/ai-agent`, request);
  }
}