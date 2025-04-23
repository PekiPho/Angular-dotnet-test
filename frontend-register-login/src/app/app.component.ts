import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { LoginComponent } from '../start/login/login.component';
import { RegisterComponent } from '../start/register/register.component';
import { GetstartedComponent } from "./getstarted/getstarted.component";
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { UserServiceService } from '../services/user-service.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'frontend-register-login';

  constructor(private userService:UserServiceService,private route:Router){}

  //private firstLoad:boolean=true;

  ngOnInit(): void {
    this.userService.getEntry().subscribe({
      next:(data)=>{
        if(this.route.url.includes('login') || this.route.url.includes('getStarted'))
          this.route.navigate(['./mainPage']);
                   
      },
      error:(err)=>{
        
      },
      complete:()=>{

      }
    });
  }
}
