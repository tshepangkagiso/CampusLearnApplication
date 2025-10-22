import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-student-messages',
  imports: [RouterLink],
  templateUrl: './student-messages.html',
  styleUrl: './student-messages.css'
})
export class StudentMessages {
onLogout() {
throw new Error('Method not implemented.');
}

}
