import { Routes, withDebugTracing } from '@angular/router';
import { LoginComponent } from '../start/login/login.component';
import { provideRouter } from '@angular/router';
import { GetstartedComponent } from './getstarted/getstarted.component';
import { RegisterComponent } from '../start/register/register.component';
import { ApplicationConfig } from '@angular/core';
import { MainPageComponent } from '../main-page/main-page/main-page.component';
import { CommunityComponent } from '../post-list/community/community.component';
import { PostComponent } from '../post-list/post/post.component';
import { ProfileComponent } from '../post-list/profile/profile.component';
import { PostListComponent } from '../post-list/post-list.component';
import { PostDetailComponent } from '../post-list/post-detail/post-detail.component';
import { SearchComponent } from '../post-list/search/search.component';

export const routes: Routes = [
    {
        path:"",
        pathMatch:'full',
        redirectTo: '/getStarted'
    },
    {
        path:'getStarted',
        component:GetstartedComponent
    },
    {
        path:'login',
        component:LoginComponent
        // loadComponent:()=>{
        //     return import('../login/login.component').then((m)=>m.LoginComponent);
        // }
    },
    {
        path:'register',
        component:RegisterComponent
        // loadComponent:()=>{
        //     return import('../register/register.component').then((m)=>m.RegisterComponent);
        // }
    },
    {
        path:'mainPage',
        component:MainPageComponent,
        //data:{refreshComponent:true},
        children:[
            {path:'',component:PostListComponent},
            { path: 'community/:name', component: CommunityComponent },
            { path: 'community/:name/post/:postID', component: PostDetailComponent ,pathMatch:'full'},
            { path: 'profile/:userID', component: ProfileComponent},
            { path: 'search/:query',component:SearchComponent},
        ]
    }
    //{ path: '**', redirectTo: 'mainPage' }

];

export const appConfig: ApplicationConfig = {
    providers: [provideRouter(routes, withDebugTracing())]
}
