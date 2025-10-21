import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateTutorResponseRequest, UpdateTutorResponseRequest } from '../../models/Topics Related Models/topics-request.dtos';
import { TutorQueriesResponse, QueryTopicResponse, QueryResponseResponse, QueryResponsesResponse } from '../../models/Topics Related Models/topics-response.dtos';


@Injectable({
  providedIn: 'root'
})
export class TutorTopicsService {
  private baseUrl = `${environment.apiUrl}/topics`;

  constructor(private http: HttpClient) {}

  // Get all queries assigned to tutor
  getTutorQueries(tutorId: number): Observable<TutorQueriesResponse> {
    return this.http.get<TutorQueriesResponse>(`${this.baseUrl}/tutors/${tutorId}/queries`);
  }

  // Get all queries (for admin/moderation)
  getAllQueries(): Observable<QueryTopicResponse[]> {
    return this.http.get<QueryTopicResponse[]>(`${this.baseUrl}/queries`);
  }

  // Create tutor response with optional media
  createTutorResponse(queryTopicId: number, tutorId: number, request: CreateTutorResponseRequest): Observable<any> {
    const formData = new FormData();
    formData.append('Comment', request.comment);
    formData.append('IsSolution', request.isSolution.toString());
    
    if (request.mediaContent) {
      formData.append('MediaContent', request.mediaContent);
    }

    return this.http.post(`${this.baseUrl}/querytopics/${queryTopicId}/tutors/${tutorId}/responses`, formData);
  }

  // Update tutor response
  updateTutorResponse(responseId: number, tutorId: number, request: UpdateTutorResponseRequest): Observable<QueryResponseResponse> {
    const formData = new FormData();
    formData.append('Comment', request.comment);
    formData.append('IsSolution', request.isSolution.toString());
    
    if (request.mediaContent) {
      formData.append('MediaContent', request.mediaContent);
    }

    return this.http.put<QueryResponseResponse>(`${this.baseUrl}/responses/${responseId}/tutors/${tutorId}`, formData);
  }

  // Get all responses for a query topic
  getQueryResponses(queryTopicId: number): Observable<QueryResponsesResponse> {
    return this.http.get<QueryResponsesResponse>(`${this.baseUrl}/querytopics/${queryTopicId}/responses`);
  }

  // Get specific response by ID
  getResponseById(responseId: number): Observable<QueryResponseResponse> {
    return this.http.get<QueryResponseResponse>(`${this.baseUrl}/responses/${responseId}`);
  }

  // Delete a response
  deleteResponse(responseId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/responses/${responseId}`);
  }

  // Download media file from response
  getResponseFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/file/${fileName}`, { responseType: 'blob' });
  }
}