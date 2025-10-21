import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { SubscribeModuleRequest } from '../../models/Student Related Models/student-request.dtos';
import { AssignedTutorResponse, AvailableModulesResponse, StudentSubscriptionsResponse, SubscriptionResponse } from '../../models/Student Related Models/student-response.dtos';

@Injectable({
  providedIn: 'root'
})
export class StudentSubscriptionService {
  private baseUrl = `${environment.apiUrl}/users/subscriptions`;

  constructor(private http: HttpClient) {}

  // Subscribe to a module
  subscribeToModule(request: SubscribeModuleRequest): Observable<SubscriptionResponse> {
    return this.http.post<SubscriptionResponse>(`${this.baseUrl}/module/student/subscribe`, request);
  }

  // Unsubscribe from a module
  unsubscribeFromModule(request: SubscribeModuleRequest): Observable<SubscriptionResponse> {
    return this.http.post<SubscriptionResponse>(`${this.baseUrl}/module/student/unsubscribe`, request);
  }

  // Get all modules student is subscribed to
  getSubscribedModules(studentId: number): Observable<StudentSubscriptionsResponse> {
    return this.http.get<StudentSubscriptionsResponse>(`${this.baseUrl}/module/students/subscribed/${studentId}`);
  }

  // Get all modules student is NOT subscribed to (available for subscription)
  getAvailableModules(studentId: number): Observable<AvailableModulesResponse> {
    return this.http.get<AvailableModulesResponse>(`${this.baseUrl}/module/students/notsubscribed/${studentId}`);
  }

  // Get assigned tutor for a specific module
  getAssignedTutor(moduleCode: string): Observable<AssignedTutorResponse> {
    return this.http.get<AssignedTutorResponse>(`${this.baseUrl}/modules/${moduleCode}/assigned-tutor`);
  }
}