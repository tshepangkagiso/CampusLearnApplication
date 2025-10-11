import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private http = inject(HttpClient);
  private basicUrl = "http://localhost:7000"


  onUpdateProfile()
  {
    
  }
}
