import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateQueryRequest, UpdateQueryRequest, CreateStudentResponseRequest } from '../../models/Topics Related Models/topics-request.dtos';
import { QueryTopicResponse, StudentQueriesResponse, QueryResponsesResponse, QueryResponseResponse } from '../../models/Topics Related Models/topics-response.dtos';


@Injectable({
  providedIn: 'root'
})
export class StudentTopicsService {
  private baseUrl = `${environment.apiUrl}/topics`;
  private baseUrlQuery = `${this.baseUrl}/QueryTopics`;
  private baseUrlResponse = `${this.baseUrl}/QueryResponses`;

  constructor(private http: HttpClient) {}


  //requests

  // Create a new query topic
  createQuery(request: CreateQueryRequest): Observable<QueryTopicResponse> {
    return this.http.post<QueryTopicResponse>(`${this.baseUrlQuery}/queries`, request);
  }

  // Get all queries by student
  getStudentQueries(studentId: number): Observable<StudentQueriesResponse> {
    return this.http.get<StudentQueriesResponse>(`${this.baseUrlQuery}/students/${studentId}/queries`);
  }

  // Get specific query by ID
  getQueryById(queryId: number): Observable<QueryTopicResponse> {
    return this.http.get<QueryTopicResponse>(`${this.baseUrlQuery}/queries/${queryId}`);
  }

  // Update a query (mark as resolved, update details)
  updateQuery(queryId: number, request: UpdateQueryRequest): Observable<QueryTopicResponse> {
    return this.http.put<QueryTopicResponse>(`${this.baseUrlQuery}/queries/${queryId}`, request);
  }

  // Delete a query
  deleteQuery(queryId: number): Observable<any> {
    return this.http.delete(`${this.baseUrlQuery}/queries/${queryId}`);
  }




  //Responses

  // Create a student response to a query
  createStudentResponse(queryTopicId: number, studentId: number, request: CreateStudentResponseRequest): Observable<any> {
    return this.http.post(`${this.baseUrlResponse}/querytopics/${queryTopicId}/students/${studentId}/responses`, request);
  }

  // Get all responses for a query topic
  getQueryResponses(queryTopicId: number): Observable<QueryResponsesResponse> {
    return this.http.get<QueryResponsesResponse>(`${this.baseUrlResponse}/querytopics/${queryTopicId}/responses`);
  }

  // Get specific response by ID
  getResponseById(responseId: number): Observable<QueryResponseResponse> {
    return this.http.get<QueryResponseResponse>(`${this.baseUrlResponse}/responses/${responseId}`);
  }

  // Delete a response
  deleteResponse(responseId: number): Observable<any> {
    return this.http.delete(`${this.baseUrlResponse}/responses/${responseId}`);
  }

  // Download media file from response
  getResponseFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrlResponse}/file/${fileName}`, { responseType: 'blob' });
  }
}