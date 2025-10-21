import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { SubscribeModuleRequest } from '../../models/Student Related Models/student-request.dtos';
import { TutorQualificationResponse, TutorQualificationsResponse, ModuleTutorsResponse, AvailableTutorModulesResponse } from '../../models/Tutor Related Models/tutor-response.dtos';


@Injectable({
  providedIn: 'root'
})
export class TutorQualificationService {
  private baseUrl = `${environment.apiUrl}/users/Subscriptions`;

  constructor(private http: HttpClient) {}

  // Qualify for a module
  qualifyForModule(request: SubscribeModuleRequest): Observable<TutorQualificationResponse> {
    return this.http.post<TutorQualificationResponse>(`${this.baseUrl}/module/tutor/subscribe`, request);
  }

  // Remove qualification from a module
  removeQualification(request: SubscribeModuleRequest): Observable<TutorQualificationResponse> {
    return this.http.post<TutorQualificationResponse>(`${this.baseUrl}/module/tutor/unsubscribe`, request);
  }

  // Get all modules tutor is qualified for
  getQualifiedModules(tutorId: number): Observable<TutorQualificationsResponse> {
    return this.http.get<TutorQualificationsResponse>(`${this.baseUrl}/module/tutors/subscribed/${tutorId}`);
  }

  // Get all tutors qualified for a specific module
  getTutorsForModule(moduleCode: string): Observable<ModuleTutorsResponse> {
    return this.http.get<ModuleTutorsResponse>(`${this.baseUrl}/module/${moduleCode}/tutors`);
  }


  getModulesTutorNotSubscribedTo(tutorId: number): Observable<AvailableTutorModulesResponse> {
    return this.http.get<AvailableTutorModulesResponse>(`${this.baseUrl}/module/tutors/notsubscribed/${tutorId}`);
  }
}