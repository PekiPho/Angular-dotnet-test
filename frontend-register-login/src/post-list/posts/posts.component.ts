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
  imports: [PostComponent, NgIf, NgFor],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent implements OnInit,OnChanges,OnDestroy{

  public posts:Post[]=[];

  communityName:string='';
 
  limit:number=50;
  page:number=1;

  public isLoading:boolean=false;
  public hasMore:boolean=true;
  
  
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

        this.sortBy = "Hot";
        this.age = "Today";
        this.sortByChange.emit(this.sortBy);
        this.ageChange.emit(this.age);

        this.resetPosts();
        this.loadPosts();
      }
    })


    if(this.communityName)
      this.loadPosts();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['age'] || changes['sortBy']){
      this.resetPosts();
      this.loadPosts();
    }
  }

  ngOnDestroy(): void {
    if(this.routerSubscription){
      this.routerSubscription.unsubscribe();
    }
  }

  loadComm(){
    const url=this.route.url.split('/');
    var newComm=url[url.length-1];

    if(newComm!==this.communityName){
      this.communityName=newComm;
      this.resetPosts();
    }
  }

  resetPosts(){
    this.posts=[];
    this.page=1;
    this.hasMore=true;
    this.isLoading=false;
  }

  loadPosts(){
    if(this.communityName=='')
      return;

    

    if(this.isLoading || !this.hasMore) return;
    this.isLoading=true;

    
    //console.log(this.isLoading, this.hasMore);

    this.postService.getPostsBySort(this.communityName,this.sortBy,this.page,this.limit,this.age)
    //testing this
    
    //works until here
    .subscribe({
      next:(data)=>{
        if(data.length<this.limit)
          this.hasMore=false;

        this.posts=[...this.posts,...data];

        // this.posts.forEach(post=>{
        //   console.log(post.vote);
        // });
        this.page++;
        //console.log(this.isLoading, this.hasMore);
        this.loadMedia();
        this.isLoading=false;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
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
    
  }

  onScroll(event:Event){
    const element = event.target as HTMLElement;
    const threshold = 150;
    const scrollTop = element.scrollTop;
    const clientHeight = element.clientHeight;
    const scrollHeight = element.scrollHeight;

    //console.log(scrollTop + clientHeight >= scrollHeight - threshold)
    if (scrollTop + clientHeight >= scrollHeight - threshold && !this.isLoading && this.hasMore) {
      this.loadPosts();
    }
  }
 
}
