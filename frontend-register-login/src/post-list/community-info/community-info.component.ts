import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { CommunityService } from '../../services/community.service';
import { Community, CommunityToSend } from '../../interfaces/community';
import { SubscribeService } from '../../services/subscribe.service';
import { UserServiceService } from '../../services/user-service.service';
import { User, UserWithoutPass } from '../../interfaces/user';
import { forkJoin } from 'rxjs';
import { NgFor, NgIf } from '@angular/common';
import { RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'community-info',
  imports: [NgIf,NgFor,RouterLink,RouterModule],
  templateUrl: './community-info.component.html',
  styleUrl: './community-info.component.scss'
})
export class CommunityInfoComponent implements OnInit{

  public subs:number=-1;
  public community:Community | null=null;
  public isModerating:boolean=false;
  public user:User | null=null;
  public username:string='';
  public moderators:UserWithoutPass[] | null =null;

  constructor(private communityService:CommunityService,private subscribeService:SubscribeService,private userService:UserServiceService){}

  
  public editCommunity:boolean=false;
  public editModerators:boolean=false;
  public isAdd:boolean=true;
  public modSecond:number=0;
  public ban:boolean=true;

  ngOnInit(): void {
    this.userService.userr$.subscribe({
      next: (userData) => {
        this.user = userData;
        //console.log(this.user);

        this.communityService.fullCommunity$.subscribe({
          next: (communityData) => {
            this.community = communityData;
            //console.log(this.community);

      
            this.loadModeratorsAndSubscriptionStatus();
          },
          error: (err) => {
            console.log(err);
          },
          complete: () => {}
        });
        this.communityService.subCount$.subscribe({
          next: (subCountData) => {
            this.subs = subCountData;
            //console.log(this.subs);
          },
          error: (err) => {
            console.log(err);
          },
          complete: () => {}
        });
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {}
    });
  }

  private loadModeratorsAndSubscriptionStatus(): void {
    if (!this.user || !this.community) return; 

    forkJoin({
      isModerating: this.subscribeService.isUserModerating(this.user.username, this.community.name),
      moderators: this.subscribeService.findModerators(this.community.name)
    }).subscribe({
      next: (data) => {
        this.isModerating = data.isModerating as boolean;
        this.moderators = data.moderators ? data.moderators.map((moderator: any) => ({
          id: moderator.id,
          username: moderator.username,
          email: moderator.email
        })):[];

        //console.log( this.isModerating);
        //console.log( this.moderators);
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {
      }
    });
  }

  addModerator(){

  }

  removeModerator(){
    
  }


  setModerator(value:boolean){
    this.editModerators=value;
    //console.log(this.editModerators);
  }

  setCommunityEdit(value:boolean){
    this.editCommunity=value;
    //console.log(this.editCommunity)
  }

  setAdd(value:boolean){
    this.isAdd=value;
  }

  setEdit(value:number){
    this.modSecond=value;
  }

  setBan(value:boolean){
    this.ban=value;
  }
}


