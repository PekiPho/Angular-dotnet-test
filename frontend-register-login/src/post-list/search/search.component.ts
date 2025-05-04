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

  ngOnInit(): void {
    var seg=this.route.url.split('/');
    this.query=seg[seg.length-1];
    this.search();
    
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
    if(this.routerSub){
      this.routerSub.unsubscribe();
    }
  }

  

  search(){
    this.postService.fullSearch(this.query).subscribe({
      next:(data)=>{
        this.communities=data.communities;
        this.posts=data.posts;

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
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }
}
