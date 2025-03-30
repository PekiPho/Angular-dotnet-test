import { Component, OnInit } from '@angular/core';
import { NavSearchComponent } from '../nav-search/nav-search.component';
import { UserServiceService } from '../../services/user-service.service';
import { CommunitiesComponent } from "../communities/communities.component";
import { CommunityService } from '../../services/community.service';
import { Community } from '../../interfaces/community';
import { NgIf } from '@angular/common';
import { RouterModule, RouterOutlet } from '@angular/router';
import { User } from '../../interfaces/user';

@Component({
  selector: 'main-page',
  imports: [NavSearchComponent, CommunitiesComponent,NgIf,RouterOutlet,RouterModule],
  templateUrl: './main-page.component.html',
  styleUrl: './main-page.component.scss'
})
export class MainPageComponent implements OnInit {
  constructor(private userService:UserServiceService,private communityService:CommunityService){}

  public user:string='';
  public id:number=-1;

  public communities:Community[]=[];
  public UserSmall:User | null=null;

  ngOnInit(): void {
    this.getUsername();
    
  }
  getCommunities() {
    //console.log(this.id);
    this.communityService.getCommunity(this.id).subscribe({
      next:(data)=>{
        //console.log(data);
        if(Array.isArray(data)){
          this.communities=data.map((community:any)=>({  
            ...community
        }));
        
        //console.log(data);
        //console.log(this.communities);
        //console.log(this.communities[0].id);
      }},
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{
        console.log("Complete");
      }
    });
  }

  getUsername(){
    this.userService.getEntry().subscribe({
      next:(data)=>{
        var dejta=JSON.parse(data);
        this.id=dejta.id;
        this.user=dejta.username;
        this.UserSmall ={
          username:dejta.username,
          email:dejta.email,
          password:dejta.password
        };
        this.userService.setUser(this.UserSmall);
        //console.log(this.id);
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{
        //console.log('User fetch complete');
        this.getCommunities();
      }
    })
  }
}
