import { Component, OnInit } from '@angular/core';
import { HistoryComponent } from '../history/history.component';
import { PostsComponent } from '../posts/posts.component';
import { CommunityInfoComponent } from "../community-info/community-info.component";
import { Community, CommunityToSend } from '../../interfaces/community';
import { CommunityService } from '../../services/community.service';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
import { BehaviorSubject } from 'rxjs';
import { PostService } from '../../services/post.service';

@Component({
  selector: 'community',
  imports: [ PostsComponent, CommunityInfoComponent,NgIf],
  templateUrl: './community.component.html',
  styleUrl: './community.component.scss'
})
export class CommunityComponent implements OnInit{

  public community:Community | null=null;

  public communitySending={} as CommunityToSend;

  public subs:number = -1;

  constructor(private communityService:CommunityService,private activatedRoute:ActivatedRoute,private postService:PostService ){}

  ngOnInit(): void {
    this.activatedRoute.url.subscribe(url=>{
      const urlPath = url.map(c=>c.path);
      //console.log(urlPath[0]);
      if(urlPath[0]=='community'){
        this.communityService.getCommunityByName(urlPath[1]).subscribe({
          next:(data)=>{
            this.community=data as Community;
            //console.log(this.community);

            this.communitySending.name=this.community.name;
            this.communitySending.description=this.community.description;
            this.communityService.setCommunity(this.communitySending);
            //console.log(this.communitySending);
          },
          error:(err)=>{
            console.log(err);
          },
          complete:()=>{}
      });
      this.communityService.getSubscriberCount(urlPath[1]).subscribe({
        next:(data)=>{
          this.subs=data as number;
          //console.log(this.subs);
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });
      }
    });
  }

  public sort:boolean=false;


  public selectedSort:string='Hot';

  setSort(name:string){
    this.selectedSort=name;

    if(name=='Top')
      this.sort=true;
    else{
      this.sort=false;
      this.selectedTime='Today';
    }
  }

  public selectedTime:string='Today';

  setTime(name:string){
    this.selectedTime=name;
  }
}
