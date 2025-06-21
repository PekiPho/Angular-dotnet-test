import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { HistoryComponent } from '../history/history.component';
import { NavigationEnd, Router } from '@angular/router';
import { PostService } from '../../services/post.service';
import { Community } from '../../interfaces/community';
import { Post } from '../../interfaces/post';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';
import { PostComponent } from '../post/post.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'search',
  imports: [HistoryComponent,NgIf,NgFor,PostComponent],
  templateUrl: './search.component.html',
  styleUrl: './search.component.scss'
})
export class SearchComponent implements OnInit,OnDestroy{

  constructor(private route:Router,
    private postService:PostService,
    private communityService:CommunityService,
  ){}

  public communities:Community[]=[];
  public posts:Post[]=[];
  public subs:{[key:string]:number}={};
  private query:string='';
  private routerSub:Subscription=new Subscription();
  public isLoading:boolean=false;
  public hasMore:boolean=true;
  public currentPage:number=1;

  ngOnInit(): void {
    var seg=this.route.url.split('/');
    this.query=seg[seg.length-1];
    this.search();

    //window.addEventListener('scroll', this.onScroll.bind(this));
    
    this.routerSub=this.route.events.subscribe((event)=>{
      //console.log("hiii");
      if(event instanceof NavigationEnd){
        //console.log("hello");
        var newSeg=this.route.url.split('/');
        this.query=newSeg[newSeg.length-1];
        this.search();
      }
    })

  }

  ngOnDestroy(): void {
    //window.removeEventListener('scroll', this.onScroll.bind(this));

    if(this.routerSub){
      this.routerSub.unsubscribe();
    }
  }

  

  search(page:number=1){
    console.log(this.isLoading, this.hasMore);
    if(this.isLoading || !this.hasMore) return;

    this.isLoading=true;

    this.postService.fullSearch(this.query,page).subscribe({
      next:(data)=>{
        if(page===1){
          this.communities=data.communities;
          this.posts=data.posts;
        }
        else{
          this.posts=[...this.posts,...data.posts];
        }

        this.hasMore= data.posts.length===50;
        
        if(page == 1){
          this.communities.forEach((community)=>{
            this.communityService.getSubscriberCount(community.name).subscribe({
              next:(data)=>{
                this.subs[community.name]=data as number;
              },
              error:(err)=>{
                console.log(err);
              }
            })
          })
        }

        this.isLoading=false;
        this.currentPage=page;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
      }
    });
  }

  onScroll(event:Event){
    var threshold=150;
    var element= event.target as HTMLElement;

    var Top=element.scrollTop;
    var height=element.clientHeight;
    var scrollHeight=element.scrollHeight;

    if(scrollHeight + height >= scrollHeight-threshold && !this.isLoading && this.hasMore){
      this.search(this.currentPage+1);
    }
  }
}
