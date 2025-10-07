import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

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
    return this.httpClient.get<string>(`${this.baseUrl}/topic/topic`, { responseType: 'text' as 'json' });
  }

  call_forum(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/forum/forum`, { responseType: 'text' as 'json' });
  }

  call_media(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/media/media`, { responseType: 'text' as 'json' });
  }

  call_messages(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/messages/messages`, { responseType: 'text' as 'json' });
  }

  call_notifications(): Observable<string> {
    return this.httpClient.get<string>(`${this.baseUrl}/notifications/notifications`, { responseType: 'text' as 'json' });
  }
}
