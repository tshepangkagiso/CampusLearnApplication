import { HttpClient } from '@angular/common/http';
import { inject, Injectable, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth/auth-service';

@Injectable({
  providedIn: 'root'
})
export class AppService {

  private httpClient = inject(HttpClient);
  
  private baseUrl = 'http://localhost:7000';


  call_users(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/user/users`, { responseType: 'text' as 'json' });
  }

  call_topics(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/topics/topic`, { responseType: 'text' as 'json' });
  }

  call_forum(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/forums/forum`, { responseType: 'text' as 'json' });
  }

  call_media(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/medias/media`, { responseType: 'text' as 'json' });
  }

  call_messages(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/messages/message`, { responseType: 'text' as 'json' });
  }

  call_notifications(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/notifications/notification`, { responseType: 'text' as 'json' });
  }


}
