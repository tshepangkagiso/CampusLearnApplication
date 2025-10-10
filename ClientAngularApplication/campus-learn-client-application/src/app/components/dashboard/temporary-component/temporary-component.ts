import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth/auth-service';
import { AppService } from '../../../services/app-service';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-temporary-component',
  imports: [DatePipe, RouterLink],
  templateUrl: './temporary-component.html',
  styleUrl: './temporary-component.css'
})
export class TemporaryComponent {
  private appService = inject(AppService);
  private authService = inject(AuthService);
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

  onLoggedOut()
  {
    this.authService.onLogout();
  }
}
