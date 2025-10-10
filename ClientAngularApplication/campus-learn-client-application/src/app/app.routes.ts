import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login-component/login-component';
import { RegistrationComponent } from './components/auth/registration-component/registration-component';
import { App } from './app';
import { AuthGuardService } from './services/auth/auth-guard-service';
import { TemporaryComponent } from './components/dashboard/temporary-component/temporary-component';

export const routes: Routes = [
    {path:'login', component: LoginComponent, title:'login page'},
    {path:'register', component: RegistrationComponent, title:'registration page'},

    //protected routes
    {path:'', component: App, title:'home', canActivate:[AuthGuardService]},
    {path:'temp', component: TemporaryComponent, title:'temp dash', canActivate:[AuthGuardService]}
];
