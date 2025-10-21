import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateFAQRequest, UpdateFAQRequest } from '../../models/Tutor Related Models/tutor-request.dtos';
import { FAQResponse } from '../../models/Tutor Related Models/tutor-response.dtos';

@Injectable({
  providedIn: 'root'
})
export class TutorFAQService {
  private baseUrl = `${environment.apiUrl}/faqs`;

  constructor(private http: HttpClient) {}

  // Create FAQ
  createFAQ(tutorId: number, request: CreateFAQRequest): Observable<any> {
    return this.http.post(`${this.baseUrl}/tutors/${tutorId}/faqs`, request);
  }

  // Get all FAQs created by tutor
  getTutorFAQs(tutorId: number): Observable<FAQResponse[]> {
    return this.http.get<FAQResponse[]>(`${this.baseUrl}/tutors/${tutorId}/faqs`);
  }

  // Get all FAQs (including drafts) by tutor
  getAllTutorFAQs(tutorId: number): Observable<FAQResponse[]> {
    return this.http.get<FAQResponse[]>(`${this.baseUrl}/tutors/${tutorId}/faqs/all`);
  }

  // Update FAQ
  updateFAQ(faqId: number, tutorId: number, request: UpdateFAQRequest): Observable<FAQResponse> {
    return this.http.put<FAQResponse>(`${this.baseUrl}/faqs/${faqId}/tutors/${tutorId}`, request);
  }

  // Delete FAQ
  deleteFAQ(faqId: number, tutorId: number): Observable<any> {
    return this.http.delete(`${this.baseUrl}/faqs/${faqId}/tutors/${tutorId}`);
  }

  // Get FAQs by module
  getModuleFAQs(moduleCode: string): Observable<FAQResponse[]> {
    return this.http.get<FAQResponse[]>(`${this.baseUrl}/modules/${moduleCode}/faqs`);
  }
}