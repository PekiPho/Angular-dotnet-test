import { Component } from '@angular/core';
import { NavigationBarComponent } from "../navigation-bar/navigation-bar.component";
import { User } from '../../interfaces/user';
import { UserServiceService } from '../../services/user-service.service';
import { Router } from '@angular/router';
import { NgIf } from '@angular/common';
import { json } from 'express';

@Component({
  selector: 'register',
  imports: [NavigationBarComponent,NgIf],
  templateUrl: './register.component.html',
  styleUrl: './register.component.scss'
})
export class RegisterComponent {

  public used:boolean;
  constructor(private router:Router,private userService: UserServiceService){
    this.used=true;
  }

  addUser(){
    let username= (document.querySelector("#username") as HTMLInputElement).value;
    let mail= (document.querySelector("#email") as HTMLInputElement).value;
    let pass = (document.querySelector("#pass") as HTMLInputElement).value;

    let user:User={
      username:username ,
      email:mail,
      password:pass
    };

    this.userService.createUser(user).subscribe({
      next:(data)=>{
        console.log(data);
        this.used=true;

        var fullData=JSON.parse(data);
        localStorage.setItem('token',fullData.token);
        localStorage.setItem('expiration',fullData.expiration);
      },
      error:(err)=>{
        if(err.status==500){
          this.used=false;
        }
        username='';
        mail='';
        pass='';
      },
      complete:()=>{
        this.router.navigate(['./login']);
      }
    });
  }
}
