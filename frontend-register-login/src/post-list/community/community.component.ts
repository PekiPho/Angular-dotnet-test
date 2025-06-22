import { Component, OnChanges, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { PostsComponent } from '../posts/posts.component';
import { CommunityInfoComponent } from "../community-info/community-info.component";
import { Community, CommunityToSend } from '../../interfaces/community';
import { CommunityService } from '../../services/community.service';
import { ActivatedRoute } from '@angular/router';
import { NgIf } from '@angular/common';
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

    this.activatedRoute.paramMap.subscribe(url=>{
      const urlPath = url.get('name');
      //console.log(url);
      //console.log(urlPath);
      if(!urlPath) return;
      //if(urlPath[0]=='community'){
        this.communityService.getCommunityByName(urlPath).subscribe({
          next:(data)=>{
            this.community=data as Community;
            this.communityService.setFullCommunity(this.community);
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
      this.communityService.getSubscriberCount(urlPath).subscribe({
        next:(data)=>{
          this.subs=data as number;
          this.communityService.setSubCount(this.subs);
          //console.log(this.subs);
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });
      //}
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

  onSortChange(newSort:string){
    this.selectedSort=newSort;
  }
  onAgeChange(newAge:string){
    this.sort=false;    
    this.selectedTime=newAge;
  }

  @ViewChild('postsComp') postsComponent!: PostsComponent;

  onScroll(event:Event){
    //console.log("haiii");
    //console.log(event);

    const element = event.target as HTMLElement;
  const threshold = 150;
  const scrollTop = element.scrollTop;
  const clientHeight = element.clientHeight;
  const scrollHeight = element.scrollHeight;

  if (scrollTop + clientHeight >= scrollHeight - threshold) {
    //console.log(!this.postsComponent.isLoading,this.postsComponent.hasMore);
    if(this.postsComponent && !this.postsComponent.isLoading && this.postsComponent.hasMore) {

      this.postsComponent.loadPosts();
    }}

  }
}
