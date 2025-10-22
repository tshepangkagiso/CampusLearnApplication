import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login-component/login-component';
import { RegistrationComponent } from './components/auth/registration-component/registration-component';
import { App } from './app';
import { AuthGuardService } from './services/auth/guards/auth-guard-service';
import { AdminComponent } from './components/dashboard/admin-component/admin-component';
import { TutorComponent } from './components/dashboard/tutor-component/tutor-component';
import { StudentComponent } from './components/dashboard/student-component/student-component';
import { RestrictedComponent } from './components/auth/restricted-component/restricted-component';
import { AuthGuardAdminService } from './services/auth/guards/auth-guard-admin-service';
import { AuthGuardTutorService } from './services/auth/guards/auth-guard-tutor-service';
import { AuthGuardStudentService } from './services/auth/guards/auth-guard-student-service';
import { StudentTopics } from './components/dashboard/student-component/student-topics/student-topics';
import { StudentForums } from './components/dashboard/student-component/student-forums/student-forums';
import { StudentMessages } from './components/dashboard/student-component/student-messages/student-messages';
import { StudentChatbot } from './components/dashboard/student-component/student-chatbot/student-chatbot';
import { TutorForums } from './components/dashboard/tutor-component/tutor-forums/tutor-forums';
import { TutorTopics } from './components/dashboard/tutor-component/tutor-topics/tutor-topics';
import { TutorMessages } from './components/dashboard/tutor-component/tutor-messages/tutor-messages';
import { StudentFaq } from './components/dashboard/student-component/student-faq/student-faq';
import { TutorFaq } from './components/dashboard/tutor-component/tutor-faq/tutor-faq';

export const routes: Routes = [
    {path:'login', component: LoginComponent, title:'Login - CampusLearn™'},
    {path:'register', component: RegistrationComponent, title:'Registration - CampusLearn™'},
    {path: 'restricted', component:RestrictedComponent, title:'Restricted - CampusLearn™'},

    //protected routes
    {path:'', component: App, title:'Home - CampusLearn™', canActivate:[AuthGuardService]},
    {path:'admin', component: AdminComponent, title:'Admin Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardAdminService]},

    //Student related protected routes
    {path:'student', component: StudentComponent, title:'Student Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},
    {path:'student/topics', component: StudentTopics, title:'Student Topics - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},
    {path:'student/forums', component: StudentForums, title:'Student Forums - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},
    {path:'student/messages', component: StudentMessages, title:'Student Messages - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},
    {path:'student/chatbot', component: StudentChatbot, title:'Student Chatbot - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},
    {path:'student/faqs', component: StudentFaq, title:'Student FAQS - CampusLearn™', canActivate:[AuthGuardService , AuthGuardStudentService]},

    //Tutor related protected routes
    {path:'tutor', component: TutorComponent, title:'Tutor Dashboard - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
    {path:'tutor/topics', component: TutorTopics , title:'Tutor Topics - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
    {path:'tutor/forums', component: TutorForums, title:'Tutor Forums - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
    {path:'tutor/messages', component: TutorMessages, title:'Tutor Messages - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
    {path:'tutor/faqs', component: TutorFaq, title:'Tutor FAQS - CampusLearn™', canActivate:[AuthGuardService , AuthGuardTutorService]},
];
