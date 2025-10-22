import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateForumTopicRequest, UpdateForumTopicRequest, CreateForumResponseRequest, UpdateForumResponseRequest } from '../../models/Forum Related Models/forum-request.dtos';
import { ForumTopicListResponse, ModuleForumResponse, UserForumResponse, ForumResponsesResponse, ForumResponseResponse } from '../../models/Forum Related Models/forum-response.dtos';
import { ForumTopicResponse } from '../../models/Forum Related Models/forum.models';


@Injectable({
  providedIn: 'root'
})
export class ForumService {

private baseUrl = `${environment.apiUrl}/forums`;
  private forumTopicBaseUrl = `${this.baseUrl}/ForumTopic`;
  private forumTopicResponseBaseUrl = `${this.baseUrl}/ForumTopicResponse`;

  constructor(private http: HttpClient) {}

  // Helper method to handle $values in responses
  private handleValuesResponse<T>(response: any): T {
    if (response && response.$values !== undefined) {
      return response.$values;
    } else if (Array.isArray(response)) {
      return response as T;
    }
    return response;
  }

  // Helper method to handle nested $values in object responses
  private handleNestedValuesResponse(response: any, propertyName: string): any {
    if (response && response[propertyName] && response[propertyName].$values !== undefined) {
      return {
        ...response,
        [propertyName]: response[propertyName].$values
      };
    }
    return response;
  }

  // Topic Methods

  // Create a new forum topic
  createTopic(request: CreateForumTopicRequest): Observable<ForumTopicResponse> {
    return this.http.post<ForumTopicResponse>(`${this.forumTopicBaseUrl}/topics`, request);
  }

  // Get all forum topics
  getAllTopics(): Observable<ForumTopicListResponse> {
    return this.http.get<any>(`${this.forumTopicBaseUrl}/topics`).pipe(
      map(response => {
        console.log('Raw forum topics response:', response);
        
        const handledResponse = this.handleNestedValuesResponse(response, 'topics');
        
        if (handledResponse.topics && Array.isArray(handledResponse.topics)) {
          return {
            totalTopics: handledResponse.totalTopics || handledResponse.topics.length,
            topics: handledResponse.topics
          };
        } else if (Array.isArray(handledResponse)) {
          return {
            totalTopics: handledResponse.length,
            topics: handledResponse
          };
        } else if (handledResponse.$values) {
          return {
            totalTopics: handledResponse.$values.length,
            topics: handledResponse.$values
          };
        } else {
          console.warn('Unexpected forum topics structure:', handledResponse);
          return { totalTopics: 0, topics: [] };
        }
      }),
      catchError(error => {
        console.error('Error loading forum topics:', error);
        return of({ totalTopics: 0, topics: [] });
      })
    );
  }

  // Get specific forum topic by ID
  getTopicById(topicId: number): Observable<ForumTopicResponse> {
    return this.http.get<any>(`${this.forumTopicBaseUrl}/topics/${topicId}`).pipe(
      map(response => {
        console.log('Raw forum topic response:', response);
        
        // Handle responses array if it exists
        if (response && response.responses && response.responses.$values !== undefined) {
          return {
            ...response,
            responses: response.responses.$values
          };
        }
        return response;
      }),
      catchError(error => {
        console.error('Error loading forum topic:', error);
        throw error;
      })
    );
  }

  // Get forum topics by module
  getTopicsByModule(moduleCode: string): Observable<ModuleForumResponse> {
    return this.http.get<any>(`${this.forumTopicBaseUrl}/modules/${moduleCode}/topics`).pipe(
      map(response => {
        console.log('Raw module topics response:', response);
        
        const handledResponse = this.handleNestedValuesResponse(response, 'topics');
        
        if (handledResponse.topics && Array.isArray(handledResponse.topics)) {
          return {
            moduleCode: handledResponse.moduleCode || moduleCode,
            totalTopics: handledResponse.totalTopics || handledResponse.topics.length,
            topics: handledResponse.topics
          };
        } else if (Array.isArray(handledResponse)) {
          return {
            moduleCode: moduleCode,
            totalTopics: handledResponse.length,
            topics: handledResponse
          };
        } else {
          console.warn('Unexpected module topics structure:', handledResponse);
          return { moduleCode: moduleCode, totalTopics: 0, topics: [] };
        }
      }),
      catchError(error => {
        console.error('Error loading module topics:', error);
        return of({ moduleCode: moduleCode, totalTopics: 0, topics: [] });
      })
    );
  }

  // Get forum topics by user
  getTopicsByUser(userProfileId: number): Observable<UserForumResponse> {
    return this.http.get<any>(`${this.forumTopicBaseUrl}/users/${userProfileId}/topics`).pipe(
      map(response => {
        console.log('Raw user topics response:', response);
        
        const handledResponse = this.handleNestedValuesResponse(response, 'topics');
        
        if (handledResponse.topics && Array.isArray(handledResponse.topics)) {
          return {
            userProfileId: handledResponse.userProfileId || userProfileId,
            totalTopics: handledResponse.totalTopics || handledResponse.topics.length,
            topics: handledResponse.topics
          };
        } else if (Array.isArray(handledResponse)) {
          return {
            userProfileId: userProfileId,
            totalTopics: handledResponse.length,
            topics: handledResponse
          };
        } else {
          console.warn('Unexpected user topics structure:', handledResponse);
          return { userProfileId: userProfileId, totalTopics: 0, topics: [] };
        }
      }),
      catchError(error => {
        console.error('Error loading user topics:', error);
        return of({ userProfileId: userProfileId, totalTopics: 0, topics: [] });
      })
    );
  }

  // Update forum topic (owner only)
  updateTopic(topicId: number, userProfileId: number, request: UpdateForumTopicRequest): Observable<ForumTopicResponse> {
    return this.http.put<ForumTopicResponse>(`${this.forumTopicBaseUrl}/topics/${topicId}/users/${userProfileId}`, request);
  }

  // Delete forum topic (owner only)
  deleteTopic(topicId: number, userProfileId: number): Observable<any> {
    return this.http.delete(`${this.forumTopicBaseUrl}/topics/${topicId}/users/${userProfileId}`);
  }

  // Upvote forum topic
  upvoteTopic(topicId: number): Observable<{ topicId: number; upvotes: number }> {
    return this.http.post<{ topicId: number; upvotes: number }>(`${this.forumTopicBaseUrl}/topics/${topicId}/upvote`, {});
  }

  // Pin/unpin topic (admin only)
  pinTopic(topicId: number, isPinned: boolean): Observable<any> {
    return this.http.post(`${this.forumTopicBaseUrl}/topics/${topicId}/pin`, isPinned);
  }

  // Lock/unlock topic (admin only)
  lockTopic(topicId: number, isLocked: boolean): Observable<any> {
    return this.http.post(`${this.forumTopicBaseUrl}/topics/${topicId}/lock`, isLocked);
  }

  // Response Methods

  // Create forum response
  createResponse(topicId: number, request: CreateForumResponseRequest): Observable<any> {
    const formData = new FormData();
    formData.append('Comment', request.comment);
    formData.append('UserProfileID', request.userProfileID.toString());
    formData.append('IsAnonymous', request.isAnonymous.toString());
    
    if (request.anonymousName) {
      formData.append('AnonymousName', request.anonymousName);
    }
    
    if (request.mediaContent) {
      formData.append('MediaContent', request.mediaContent);
    }

    return this.http.post(`${this.forumTopicResponseBaseUrl}/forumtopics/${topicId}/responses`, formData);
  }

  // Get all responses for a forum topic
  getTopicResponses(topicId: number): Observable<ForumResponsesResponse> {
    return this.http.get<any>(`${this.forumTopicResponseBaseUrl}/forumtopics/${topicId}/responses`).pipe(
      map(response => {
        console.log('Raw topic responses response:', response);
        
        const handledResponse = this.handleNestedValuesResponse(response, 'responses');
        
        if (handledResponse.responses && Array.isArray(handledResponse.responses)) {
          return {
            topicId: handledResponse.topicId || topicId,
            totalResponses: handledResponse.totalResponses || handledResponse.responses.length,
            responses: handledResponse.responses
          };
        } else if (Array.isArray(handledResponse)) {
          return {
            topicId: topicId,
            totalResponses: handledResponse.length,
            responses: handledResponse
          };
        } else {
          console.warn('Unexpected topic responses structure:', handledResponse);
          return { topicId: topicId, totalResponses: 0, responses: [] };
        }
      }),
      catchError(error => {
        console.error('Error loading topic responses:', error);
        return of({ topicId: topicId, totalResponses: 0, responses: [] });
      })
    );
  }

  // Get specific response by ID
  getResponseById(responseId: number): Observable<ForumResponseResponse> {
    return this.http.get<ForumResponseResponse>(`${this.forumTopicResponseBaseUrl}/responses/${responseId}`);
  }

  // Update forum response (owner only)
  updateResponse(responseId: number, userProfileId: number, request: UpdateForumResponseRequest): Observable<ForumResponseResponse> {
    const formData = new FormData();
    formData.append('Comment', request.comment);
    
    if (request.mediaContent) {
      formData.append('MediaContent', request.mediaContent);
    }

    return this.http.put<ForumResponseResponse>(`${this.forumTopicResponseBaseUrl}/responses/${responseId}/users/${userProfileId}`, formData);
  }

  // Delete forum response (owner only)
  deleteResponse(responseId: number, userProfileId: number): Observable<any> {
    return this.http.delete(`${this.forumTopicResponseBaseUrl}/responses/${responseId}/users/${userProfileId}`);
  }

  // Upvote forum response
  upvoteResponse(responseId: number): Observable<{ responseId: number; upvotes: number }> {
    return this.http.post<{ responseId: number; upvotes: number }>(`${this.forumTopicResponseBaseUrl}/responses/${responseId}/upvote`, {});
  }

  // File download
  getForumFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.forumTopicResponseBaseUrl}/file/${fileName}`, { responseType: 'blob' });
  }
}