import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AppService {
  private httpClient = inject(HttpClient);

  private users_management_url = 'http://localhost:6600/user/users';
  private topics_management_url = 'http://localhost:6600/topic/topic';
  private forums_management_url = 'http://localhost:6600/forum/forum';
  private media_url = 'http://localhost:6600/media/media';
  private messages_url = 'http://localhost:6600/messages/messages';
  private notifications_url = 'http://localhost:6600/notifications/notifications';

  call_users(){
    let value = "";
    this.httpClient.get<string>(this.users_management_url).subscribe({
      next: (res)=> {
        value = res;
      },
      error:(err)=>{
        console.log(err)
      }
    });
    return value;
  }

  call_topics(){
    let value = "";
     this.httpClient.get<string>(this.topics_management_url).subscribe({
      next: (res)=> {
        value = res; 
      },
      error:(err)=>{
        console.log(err)
      }
    });
    return value;   
  }

  call_forum(){
    let value = "";
     this.httpClient.get<string>(this.forums_management_url).subscribe({
      next: (res)=> {
        value = res;
      },
      error:(err)=>{
        console.log(err)
      }
    }); 
    return value;  
  }

  call_media(){
    let value = "";
    this.httpClient.get<string>(this.media_url).subscribe({
      next: (res)=> {
        value = res;
      },
      error:(err)=>{
        console.log(err)
      }
    });
    return value;
  }

  call_messages(){
    let value = "";
     this.httpClient.get<string>(this.messages_url).subscribe({
      next: (res)=> {
        value = res;
      },
      error:(err)=>{
        console.log(err)
      }
    }); 
    return value;
  }

  call_notifications(){
    let value = "";
    this.httpClient.get<string>(this.notifications_url).subscribe({
      next: (res)=> {
        value = res;
      },
      error:(err)=>{
        console.log(err)
      }
    });
    return value; 
  }
}
