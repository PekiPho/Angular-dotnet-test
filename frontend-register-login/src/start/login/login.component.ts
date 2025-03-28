import { Component,OnInit } from '@angular/core';
import { NavigationBarComponent } from "../navigation-bar/navigation-bar.component";
import { UserServiceService } from '../../services/user-service.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { HttpResponseBase } from '@angular/common/http';
import { User } from '../../interfaces/user';
import { response } from 'express';

@Component({
  selector: 'login',
  imports: [NavigationBarComponent,NgIf],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {

  public checked:boolean;

  public user:string='';

  constructor(private userService:UserServiceService,private router:Router)
  {
    this.checked=true;
  }


  checkLoginInfo(){
    let email=(document.querySelector("#email") as HTMLInputElement).value;
    let pass =(document.querySelector("#pass") as HTMLInputElement).value;

    if(pass!='' && email !=''){
      this.userService.checkLogin(email,pass).subscribe(
        {
          next:(data)=>{
            this.checked=true;
            this.user=data;
            //this.userService.sendUserToComponent(this.user);
            //console.log(this.username);
          },
          error:(err:HttpResponseBase)=>{
            console.log(err);
            if(err.status==400){
              this.checked=false;
            }
          },
          complete:()=>{
            this.router.navigate(['./mainPage']);
          }
        }
      );
    }
  
    //email='';
    //pass='';
  }

}
