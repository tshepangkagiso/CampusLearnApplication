import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateForumTopicRequest, UpdateForumTopicRequest, CreateForumResponseRequest, UpdateForumResponseRequest } from '../../models/Forum Related Models/forum-request.dtos';
import { ForumTopicListResponse, ModuleForumResponse, UserForumResponse, ForumResponsesResponse, ForumResponseResponse } from '../../models/Forum Related Models/forum-response.dtos';
import { ForumTopicResponse } from '../../models/Forum Related Models/forum.models';


@Injectable({
  providedIn: 'root'
})
export class ForumService {
  private baseUrl = `${environment.apiUrl}/forums`;

  constructor(private http: HttpClient) {}

  // Topic Methods

  // Create a new forum topic
  createTopic(request: CreateForumTopicRequest): Observable<ForumTopicResponse> {
    return this.http.post<ForumTopicResponse>(`${this.baseUrl}/topics`, request);
  }

  // Get all forum topics
  getAllTopics(): Observable<ForumTopicListResponse> {
    return this.http.get<ForumTopicListResponse>(`${this.baseUrl}/topics`);
  }

  // Get specific forum topic by ID
  getTopicById(topicId: number): Observable<ForumTopicResponse> {
    return this.http.get<ForumTopicResponse>(`${this.baseUrl}/topics/${topicId}`);
  }

  // Get forum topics by module
  getTopicsByModule(moduleCode: string): Observable<ModuleForumResponse> {
    return this.http.get<ModuleForumResponse>(`${this.baseUrl}/modules/${moduleCode}/topics`);
  }

  // Get forum topics by user
  getTopicsByUser(userProfileId: number): Observable<UserForumResponse> {
    return this.http.get<UserForumResponse>(`${this.baseUrl}/users/${userProfileId}/topics`);
  }

  // Update forum topic (owner only)
  updateTopic(topicId: number, userProfileId: number, request: UpdateForumTopicRequest): Observable<ForumTopicResponse> {
    return this.http.put<ForumTopicResponse>(`${this.baseUrl}/topics/${topicId}/users/${userProfileId}`, request);
  }

  // Delete forum topic (owner only)
  deleteTopic(topicId: number, userProfileId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/topics/${topicId}/users/${userProfileId}`);
  }

  // Upvote forum topic
  upvoteTopic(topicId: number): Observable<{ topicId: number; upvotes: number }> {
    return this.http.post<{ topicId: number; upvotes: number }>(`${this.baseUrl}/topics/${topicId}/upvote`, {});
  }

  // Pin/unpin topic (admin only)
  pinTopic(topicId: number, isPinned: boolean): Observable<any> {
    return this.http.post(`${this.baseUrl}/topics/${topicId}/pin`, isPinned);
  }

  // Lock/unlock topic (admin only)
  lockTopic(topicId: number, isLocked: boolean): Observable<any> {
    return this.http.post(`${this.baseUrl}/topics/${topicId}/lock`, isLocked);
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

    return this.http.post(`${this.baseUrl}/forumtopics/${topicId}/responses`, formData);
  }

  // Get all responses for a forum topic
  getTopicResponses(topicId: number): Observable<ForumResponsesResponse> {
    return this.http.get<ForumResponsesResponse>(`${this.baseUrl}/forumtopics/${topicId}/responses`);
  }

  // Get specific response by ID
  getResponseById(responseId: number): Observable<ForumResponseResponse> {
    return this.http.get<ForumResponseResponse>(`${this.baseUrl}/responses/${responseId}`);
  }

  // Update forum response (owner only)
  updateResponse(responseId: number, userProfileId: number, request: UpdateForumResponseRequest): Observable<ForumResponseResponse> {
    const formData = new FormData();
    formData.append('Comment', request.comment);
    
    if (request.mediaContent) {
      formData.append('MediaContent', request.mediaContent);
    }

    return this.http.put<ForumResponseResponse>(`${this.baseUrl}/responses/${responseId}/users/${userProfileId}`, formData);
  }

  // Delete forum response (owner only)
  deleteResponse(responseId: number, userProfileId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/responses/${responseId}/users/${userProfileId}`);
  }

  // Upvote forum response
  upvoteResponse(responseId: number): Observable<{ responseId: number; upvotes: number }> {
    return this.http.post<{ responseId: number; upvotes: number }>(`${this.baseUrl}/responses/${responseId}/upvote`, {});
  }

  // File download
  getForumFile(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/file/${fileName}`, { responseType: 'blob' });
  }
}