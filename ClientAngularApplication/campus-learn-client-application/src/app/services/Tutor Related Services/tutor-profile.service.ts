import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../models/environments/environment';
import { UpdateTutorProfileRequest } from '../../models/Tutor Related Models/tutor-request.dtos';
import { TutorProfileResponse } from '../../models/Tutor Related Models/tutor-response.dtos';

@Injectable({
  providedIn: 'root'
})
export class TutorProfileService {
  private baseUrl = `${environment.apiUrl}/users/userprofile`;

  constructor(private http: HttpClient) {}

  // Get tutor profile
  getTutorProfile(tutorId: number): Observable<TutorProfileResponse> {
    return this.http.get<TutorProfileResponse>(`${this.baseUrl}/${tutorId}`);
  }

  // Update tutor profile
  updateTutorProfile(request: UpdateTutorProfileRequest): Observable<any> {
    const formData = new FormData();
    formData.append('UserProfileID', request.userProfileID.toString());
    formData.append('Name', request.name);
    formData.append('Surname', request.surname);
    formData.append('Email', request.email);
    formData.append('Qualification', request.qualification.toString());
    formData.append('StudentNumber', request.studentNumber.toString());
    
    if (request.profilePicture) {
      formData.append('ProfilePicture', request.profilePicture); 
    }

    return this.http.put(`${this.baseUrl}/update`, formData);
  }

  // Get tutor's profile picture URL
  getProfilePictureUrl(tutorId: number): Observable<{ profilePictureUrl: string }> {
    return this.http.get<{ profilePictureUrl: string }>(`${this.baseUrl}/profile-picture-url/${tutorId}`);
  }

  // Download profile picture file
  getProfilePicture(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/file/${fileName}`, { responseType: 'blob' });
  }
}