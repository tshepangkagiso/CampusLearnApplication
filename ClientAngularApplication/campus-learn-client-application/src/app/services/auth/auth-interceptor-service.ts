import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root'
})
export class AuthInterceptorService implements HttpInterceptor{
  private authService = inject(AuthService)


  //pass token with every request in this application
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> 
  {
    const token = this.authService.getItem();
    if(token)
    {
      const cloned = req.clone({
        setHeaders:{ Authorization: `Bearer ${token}`
      }});

      return next.handle(cloned);
    }
    return next.handle(req);
  }
  
}
