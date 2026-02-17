import { Component, input, Input, OnInit } from '@angular/core';
import { Community, CommunityToSend } from '../../interfaces/community';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';
import { Router, RouterLink, RouterModule, RouterOutlet } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { SubscribeService } from '../../services/subscribe.service';
import { UserFull } from '../../interfaces/user';

@Component({
  selector: 'communities',
  imports: [NgFor,NgIf,RouterModule,RouterLink],
  templateUrl: './communities.component.html',
  styleUrl: './communities.component.scss'
})
export class CommunitiesComponent {

  constructor(private communityService:CommunityService,private subscribeService:SubscribeService,private router:Router){}

  @Input() userID:number=-1;

  @Input() communities:Community[]=[];

  public modal:boolean=false;

  openModal(){
    this.modal=true;

  }

  closeModal(){
    this.modal=false;

    //console.log(this.modal);
  }

  async createModal(){

      console.log("userID is:", this.userID);

      var name=(document.querySelector("#name") as HTMLInputElement).value;
      var description=(document.querySelector("#description") as HTMLTextAreaElement).value;

      var community:CommunityToSend={
        name,
        description
      };
    
      //console.log(this.userID);
      try{
        //console.log(this.userID);
        const comm=await firstValueFrom(this.communityService.createCommunity(community)) as Community;
        const sub=await firstValueFrom(this.subscribeService.subscribeToCommunity(this.userID,comm.id));
        const mod=await firstValueFrom(this.subscribeService.addToModerateCommunity(this.userID,comm.id));

        //console.log(comm,sub,mod);
        this.modal = false;
        setTimeout(() => window.location.reload(), 100);
        //window.location.reload();
        //this.communities = [...this.communities, comm];
        //this.closeModal(); 
      }catch(err){
        console.log(err);
        this.closeModal();
      }

  }

  navigateToCommunity(name:string){
    //console.log("workds");
    this.router.navigate(['/mainPage/community',name]);
  }
}
