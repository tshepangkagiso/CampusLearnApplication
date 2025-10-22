import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of, tap } from 'rxjs';
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
  createQuery(request: CreateQueryRequest, tutorId: number): Observable<QueryTopicResponse> {
    return this.http.post<QueryTopicResponse>(`${this.baseUrlQuery}/queries/${tutorId}`, request);
  }

  // Get all queries by student
  getStudentQueries(studentId: number): Observable<StudentQueriesResponse> {
    return this.http.get<any>(`${this.baseUrlQuery}/students/${studentId}/queries`).pipe(
      map(apiResponse => {
        console.log('Raw API Response:', apiResponse);
        
        // Transform the API response to match your expected structure
        return {
          studentId: apiResponse.studentId,
          totalQueries: apiResponse.totalQueries,
          queries: apiResponse.queries?.$values || []  // Extract the nested array
        };
      }),
      catchError(error => {
        console.error('Error fetching student queries:', error);
        // Return empty structure on error
        return of({
          studentId: studentId,
          totalQueries: 0,
          queries: []
        });
      })
    );
  }

  // Get specific query by ID
  getQueryById(queryId: number): Observable<QueryTopicResponse> {
    return this.http.get<any>(`${this.baseUrlQuery}/queries/${queryId}`).pipe(
      map(apiResponse => {
        console.log('Raw query API response:', apiResponse);
        
        // Transform the response to handle nested structures
        if (apiResponse.responses && apiResponse.responses.$values) {
          apiResponse.responses = apiResponse.responses.$values;
        }
        
        return apiResponse;
      }),
      catchError(error => {
        console.error('Error fetching query:', error);
        throw error;
      })
    );
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