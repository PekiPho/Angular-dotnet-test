import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, signal, SimpleChanges } from '@angular/core';
import { Post, PostToSend } from '../../interfaces/post';
import { PostComponent } from '../post/post.component';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { CommunityToSend } from '../../interfaces/community';
import { PostService } from '../../services/post.service';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';
import {InfiniteScrollDirective} from 'ngx-infinite-scroll';
import { Media } from '../../interfaces/media';
import { BehaviorSubject, distinctUntilChanged, filter, forkJoin, map, of, Subscription, switchMap, take, tap } from 'rxjs';

@Component({
  selector: 'posts',
  imports: [PostComponent,NgIf,NgFor,InfiniteScrollDirective],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent implements OnInit,OnChanges,OnDestroy{

  public posts:Post[]=[];

  communityName:string='';
  hasMorePosts:boolean=true;
  didInitialLoad:boolean=false;
  limit:number=50;
  page:number=1;
  
  loading=signal(false);
  @Input() sortBy:string="Hot";
  @Input() age:string="Today";

  @Output() sortByChange = new EventEmitter<string>();
  @Output() ageChange = new EventEmitter<string>();

  private isFirstInit:boolean=true;

  private communitySubscription: Subscription=new Subscription();

  constructor(private postService:PostService,private communityService:CommunityService,private route:Router){}

  private routerSubscription: Subscription = new Subscription();
  private scrollSubscription:Subscription= new Subscription();

  // private loadPostsTrigger = new BehaviorSubject<void>(undefined);
  // private loadPostsSub: Subscription=new Subscription();

  ngOnInit(): void {

    this.loadComm();

    this.routerSubscription= this.route.events.subscribe((event)=>{
      if(event instanceof NavigationEnd){
        this.loadComm();

        this.page=1;
        this.loadPosts();
      }
    })

    this.scrollSubscription= this.scrollEventListener();

    if(this.communityName)
      this.loadPosts();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['age'] || changes['sortBy']){
      this.page=1;
      this.loadPosts();
    }
  }

  ngOnDestroy(): void {
    if(this.routerSubscription){
      this.routerSubscription.unsubscribe();
    }

    if(this.scrollSubscription){
      this.scrollSubscription.unsubscribe();
    }
  }

  loadComm(){
    const url=this.route.url.split('/');
    this.communityName=url[url.length-1];
  }

  loadPosts(){
    if(this.communityName=='')
      return;
    this.postService.getPostsBySort(this.communityName,this.sortBy,this.page,this.limit,this.age)
    //testing this
    
    //works until here
    .subscribe({
      next:(data)=>{
        if(this.page==1)
          this.posts=data;
        else  
          this.posts=[...this.posts,...data];

        this.loadMedia();
      }

      
    })
  }

  loadMedia(){
    this.posts.forEach((post)=>{
      this.postService.getMediaFromPost(post.id).subscribe({
        next:(data)=>{
          post.mediaIds=data;
        },
        error:(err)=>{
          console.log(err);
        }
      })
    })
  }

  loadMorePosts(){
    this.page++;

    this.loadPosts();
  }

  scrollEventListener(){
    const container=document.querySelector(".content") as HTMLElement;

    return new Subscription(()=>{
      if(container){
        container.addEventListener('scroll',()=>{
          var bottom = container.scrollHeight === container.scrollTop + container.clientHeight;

          if(bottom){
            this.loadMorePosts();
          }
        })
      }
    })
  }
 
}
