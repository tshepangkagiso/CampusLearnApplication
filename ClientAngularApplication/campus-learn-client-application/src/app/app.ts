import { Component, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppService } from './services/app-service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  protected readonly title = signal('campus-learn');
  private appService = inject(AppService);

  public users? : string;
  public topics? : string;
  public forums? : string;
  public media? : string;
  public messages? : string;
  public notifications? : string;

  get user (){
    return this.users = this.appService.call_users();
  }

  get topic (){
    return this.topics = this.appService.call_topics();
  }

  get forum (){
    return this.forums = this.appService.call_forum();
  }
  
  get medias (){
    return this.messages = this.appService.call_media();
  }

  get message (){
    return this.messages = this.appService.call_messages();
  }


  get notification (){
    return this.notifications = this.appService.call_notifications();
  }


}
