import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, map, Observable, of } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { CreateFAQRequest, UpdateFAQRequest } from '../../models/Tutor Related Models/tutor-request.dtos';
import { FAQResponse } from '../../models/Tutor Related Models/tutor-response.dtos';

@Injectable({
  providedIn: 'root'
})
export class TutorFAQService {
  private baseUrl = `${environment.apiUrl}/topics/faqs`;

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
      return this.http.get<any>(`${this.baseUrl}/tutors/${tutorId}/faqs/all`).pipe(
        map(response => {
          console.log('Raw FAQ response:', response); // For debugging
          
          // Handle the actual response structure from your API
          if (response && response.faQs && Array.isArray(response.faQs.$values)) {
            return response.faQs.$values;
          } else if (Array.isArray(response)) {
            return response;
          } else {
            console.warn('Unexpected FAQ response structure:', response);
            return [];
          }
        }),
        catchError(error => {
          console.error('Error loading FAQs:', error);
          return of([]);
        })
      );
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
    return this.http.get<any>(`${this.baseUrl}/modules/${moduleCode}/faqs`).pipe(
      map(response => {
        console.log('Raw module FAQs response:', response); // Debug log
        
        // Handle different possible response structures
        if (Array.isArray(response)) {
          return response;
        } else if (response && Array.isArray(response.FAQs)) {
          return response.FAQs;
        } else if (response && Array.isArray(response.faqs)) {
          return response.faqs;
        } else if (response && response.faQs && Array.isArray(response.faQs.$values)) {
          return response.faQs.$values;
        } else {
          console.warn('Unexpected module FAQ response structure:', response);
          return [];
        }
      }),
      catchError(error => {
        console.error('Error loading module FAQs:', error);
        return of([]);
      })
    );
  }
}