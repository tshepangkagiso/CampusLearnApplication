import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AppService } from './services/app-service';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, DatePipe],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit{
  protected readonly title = signal('campus-learn');
  private appService = inject(AppService);
  users?: string;
  topics?: string;
  forums?: string;
  media?: string;
  messages?: string;
  notifications?: string;
  now = new Date();

  ngOnInit(): void {
    this.appService.call_users().subscribe(res => this.users = res);
    this.appService.call_topics().subscribe(res => this.topics = res);
    this.appService.call_forum().subscribe(res => this.forums = res);
    this.appService.call_media().subscribe(res => this.media = res);
    this.appService.call_messages().subscribe(res => this.messages = res);
    this.appService.call_notifications().subscribe(res => this.notifications = res);
  }


}
