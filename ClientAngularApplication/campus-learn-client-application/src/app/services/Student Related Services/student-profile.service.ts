import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { StudentProfileResponse } from '../../models/Student Related Models/student-response.dtos';
import { UpdateStudentProfileRequest } from '../../models/Student Related Models/student-request.dtos';
import { environment } from '../../models/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StudentProfileService {
  private baseUrl = `${environment.apiUrl}/users`;

  constructor(private http: HttpClient) {}

  // Get student profile
  getStudentProfile(studentId: number): Observable<StudentProfileResponse> {
    return this.http.get<StudentProfileResponse>(`${this.baseUrl}/${studentId}`);
  }

  // Update student profile
  updateStudentProfile(request: UpdateStudentProfileRequest): Observable<any> {
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

    return this.http.post(`${this.baseUrl}/update`, formData);
  }

  // Get student's profile picture URL
  getProfilePictureUrl(studentId: number): Observable<{ profilePictureUrl: string }> {
    return this.http.get<{ profilePictureUrl: string }>(`${this.baseUrl}/profile-picture-url/${studentId}`);
  }

  // Download profile picture file
  getProfilePicture(fileName: string): Observable<Blob> {
    return this.http.get(`${this.baseUrl}/file/${fileName}`, { responseType: 'blob' });
  }
}