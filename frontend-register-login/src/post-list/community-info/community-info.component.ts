import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { CommunityService } from '../../services/community.service';
import { Community, CommunityToSend } from '../../interfaces/community';
import { SubscribeService } from '../../services/subscribe.service';
import { UserServiceService } from '../../services/user-service.service';
import { User, UserWithoutPass } from '../../interfaces/user';
import { forkJoin, Subscription } from 'rxjs';
import { NgFor, NgIf } from '@angular/common';
import { ActivatedRoute, Router, RouterLink, RouterModule } from '@angular/router';

@Component({
  selector: 'community-info',
  imports: [NgIf,NgFor,RouterLink,RouterModule],
  templateUrl: './community-info.component.html',
  styleUrl: './community-info.component.scss'
})
export class CommunityInfoComponent implements OnInit,OnDestroy{

  public subs:number=-1;
  public community:Community | null=null;
  public isModerating:boolean=false;
  public user:User | null=null;
  public username:string='';
  public moderators:UserWithoutPass[] | null =null;

  constructor(private communityService:CommunityService,
    private subscribeService:SubscribeService,
    private userService:UserServiceService,
    private route:ActivatedRoute
  ){}

  
  public editCommunity:boolean=false;
  public editModerators:boolean=false;
  public isAdd:boolean=true;
  public modSecond:number=0;
  public ban:boolean=true;

  private communitySub:Subscription=new Subscription();

  ngOnInit(): void {
    this.userService.userr$.subscribe({
      next: (userData) => {
        this.user = userData;
        //console.log(this.user);

        this.route.url.subscribe((seg)=>{
          this.communitySub=this.communityService.getCommunityByName(seg[1]!.path).subscribe({
            next:(data)=>{
              this.community=data as Community;
              this.communityService.setFullCommunity(this.community);
  
              this.loadModeratorsAndSubscriptionStatus();
  
              this.communityService.getSubscriberCount(this.community!.name).subscribe({
                next:(data)=>{
                  this.subs=data as number;
                },
                error:(err)=>{
                  console.log(err);
                }
              });
            },
            error:(err)=>{
              console.log(err);
            }
          });
        });
        
        

        // this.communityService.fullCommunity$.subscribe({
        //   next: (communityData) => {
        //     this.community = communityData;
        //     //console.log(this.community);

      
        //     this.loadModeratorsAndSubscriptionStatus();
        //   },
        //   error: (err) => {
        //     console.log(err);
        //   },
        //   complete: () => {}
        // });
        // this.communityService.subCount$.subscribe({
        //   next: (subCountData) => {
        //     this.subs = subCountData;
        //     //console.log(this.subs);
        //   },
        //   error: (err) => {
        //     console.log(err);
        //   },
        //   complete: () => {}
        // });
      },
      error: (err) => {
        console.log(err);
      },
      complete: () => {}
    });
  }

  ngOnDestroy(): void {
    if(this.communitySub)
      this.communitySub.unsubscribe();
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
    var addMod=(document.querySelector("#mod") as HTMLInputElement).value;
    var errorP=document.querySelector(".addModText") as HTMLParagraphElement;
    if(addMod!=''){
      this.subscribeService.addModerator(this.community!.name,addMod).subscribe({
        next:(data)=>{
          errorP.textContent='';
          this.editModerators=false;

          var mod=data as UserWithoutPass;
          this.moderators?.push(mod);
        },
        error:(err)=>{
          errorP.textContent="Enter Valid Username";
        },
        complete:()=>{}
      });
    }
    else{
      errorP.textContent="Enter Valid Username";
    }
  }

  removeModerator(){
    var removeMod=(document.querySelector("#removeMod") as HTMLInputElement).value;
    var errorP=document.querySelector(".removeModText") as HTMLParagraphElement;

    if(removeMod!=''){
      errorP.textContent='';

      this.subscribeService.removeModerator(this.community!.name,removeMod).subscribe({
        next:(data)=>{
          errorP.textContent='';
          this.editModerators=false;

          var mod=data as UserWithoutPass;

          this.moderators=(this.moderators ?? []).filter(x=>x.id!=mod.id);
          //this.moderators?.push();
        },
        error:(err)=>{
          errorP.textContent='Enter Valid Username';
          console.log(err);
        },
        complete:()=>{}
      });
    }
    else{
      errorP.textContent="Enter Valid Username";
    }
  }

  editDescription(){
    var description=(document.querySelector("#editDesc") as HTMLTextAreaElement).value;
    var errorP=document.querySelector(".comm")as HTMLParagraphElement;

    if(description!=''){
    this.communityService.updateDescription(this.community!.name,description).subscribe({
      next:(data)=>{
        if(this.community!=null)
          this.community.description=description ?? '';

        this.editCommunity=false;
      },
      error:(err)=>{
        errorP.textContent='Enter Valid Description';
        console.log(err);
      },
      complete:()=>{}
    });
    }
    else{
      errorP.textContent='Enter Valid Description';
    }
  }

  editInfo(){
    var info=(document.querySelector("#editCommInfo") as HTMLTextAreaElement).value;
    var errorP=document.querySelector(".comm")as HTMLParagraphElement;

    if(info!=''){
      this.communityService.updateInfo(this.community!.name,info).subscribe({
        next:(data)=>{
          if(this.community!=null)
            this.community.communityInfo=info;

          this.editCommunity=false;
        },
        error:(err)=>{
          errorP.textContent= 'Enter Valid Info';
          console.log(err);
        },
        complete:()=>{}
      });
    }
    else{
      errorP.textContent='Enter Valid Info';
    }
  }

  banUser(){
    var ban=(document.querySelector("#banUser")as HTMLInputElement).value;
  }

  unbanUser(){
    var unban=(document.querySelector("#banUser")as HTMLInputElement).value;
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


