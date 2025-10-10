import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login-component/login-component';
import { RegistrationComponent } from './components/auth/registration-component/registration-component';
import { App } from './app';
import { AuthGuardService } from './services/auth/auth-guard-service';
import { TemporaryComponent } from './components/dashboard/temporary-component/temporary-component';
import { AdminComponent } from './components/dashboard/admin-component/admin-component';
import { TutorComponent } from './components/dashboard/tutor-component/tutor-component';
import { StudentComponent } from './components/dashboard/student-component/student-component';
import { RestrictedComponent } from './components/auth/restricted-component/restricted-component';
import { AuthGuardAdminService } from './services/auth/auth-guard-admin-service';
import { AuthGuardTutorService } from './services/auth/auth-guard-tutor-service';
import { AuthGuardStudentService } from './services/auth/auth-guard-student-service';

export const routes: Routes = [
    {path:'login', component: LoginComponent, title:'Login - CampusLearn™'},
    {path:'register', component: RegistrationComponent, title:'Registration - CampusLearn™'},
    {path: 'restricted', component:RestrictedComponent, title:'Restricted - CampusLearn™'},

    //protected routes
    {path:'', component: App, title:'Home - CampusLearn™', canActivate:[AuthGuardService]},
    {path:'temp', component: TemporaryComponent, title:'Temporary Dashboard - CampusLearn™', canActivate:[AuthGuardService]},

    {path:'admin', component: AdminComponent, title:'Admin Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardAdminService]},
    {path:'tutor', component: TutorComponent, title:'Tutor Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
    {path:'student', component: StudentComponent, title:'Student Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]}
];
