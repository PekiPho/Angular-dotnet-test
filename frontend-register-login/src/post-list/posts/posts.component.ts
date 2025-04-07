import { Component, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, signal, SimpleChanges } from '@angular/core';
import { Post, PostToSend } from '../../interfaces/post';
import { PostComponent } from '../post/post.component';
import { ActivatedRoute } from '@angular/router';
import { CommunityToSend } from '../../interfaces/community';
import { PostService } from '../../services/post.service';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';
import {InfiniteScrollDirective} from 'ngx-infinite-scroll';
import { Media } from '../../interfaces/media';
import { BehaviorSubject, distinctUntilChanged, filter, map, Subscription, switchMap, take, tap } from 'rxjs';

@Component({
  selector: 'posts',
  imports: [PostComponent,NgIf,NgFor,InfiniteScrollDirective],
  templateUrl: './posts.component.html',
  styleUrl: './posts.component.scss'
})
export class PostsComponent implements OnInit,OnChanges{

  public posts:Post[]=[];

  community={} as CommunityToSend;
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

  private communitySubscription: Subscription| null =null;

  constructor(private postService:PostService,private communityService:CommunityService){}

  ngOnInit(): void {
    //this.posts=[];
    //console.log("onInit");
    //this.resetComponent();

    this.resetComponent();
    this.initializePostLoader();
  }

  resetComponent(){

    if (this.communitySubscription) {
      this.communitySubscription.unsubscribe();
    }

    this.communitySubscription = this.communityService.communityy$
      .pipe(
        filter((comm): comm is CommunityToSend => !!comm && !!comm.name),
        distinctUntilChanged((prev, curr) => prev.name === curr.name)
      )
      .subscribe({
        next: (data) => {
          this.community = data;
          this.sortBy = "Hot";
          this.age = "Today";
          this.emitChanges();
          this.posts = [];
          this.page = 1;
          this.hasMorePosts = true;
          this.loadPostsTrigger.next();
        },
        error: (err) => console.log(err)
      });
  }

  ngOnChanges(changes: SimpleChanges): void {

    if(this.isFirstInit){
      this.isFirstInit=false;
      return;
    }

    const sortChanged = changes['sortBy'] && !changes['sortBy'].firstChange &&
                      changes['sortBy'].currentValue !== changes['sortBy'].previousValue;

  const ageChanged = changes['age'] && !changes['age'].firstChange &&
                     changes['age'].currentValue !== changes['age'].previousValue;

    if(sortChanged || ageChanged){
      this.posts=[];
      this.page=1;
      this.hasMorePosts=true;
      this.loadPostsTrigger.next();
      //console.log("Inside changes");
      //this.getPostsBySort();
      //this.resetComponent();
    }
        
  }

  ngOnDestroy(): void {
    if (this.communitySubscription) {
      this.communitySubscription.unsubscribe();
    }
    if (this.loadPostsSub) {
      this.loadPostsSub.unsubscribe();
    }

  }

  getPostss(){
    //console.log("hello from posts " + this.community.name);
    if(this.community.name){
      this.postService.getPosts(this.community.name).subscribe({
        next:(data)=>{
          this.posts=data;
          //console.log(this.posts);
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });
    }
    
  }

  private loadPostsTrigger = new BehaviorSubject<void>(undefined);
  private loadPostsSub: Subscription | null = null;

  initializePostLoader() {
    this.loadPostsSub = this.loadPostsTrigger
      .pipe(
        filter(() => !!this.community?.name && this.hasMorePosts),
        tap(() => {
          this.loading.set(true); 
          this.hasMorePosts = true; 
        }),
        switchMap(() => {
          
          if (this.community.name) {  
            return this.postService.getPostsBySort(
              this.community.name,
              this.sortBy,
              this.page,
              this.limit,
              this.age
            );

        }
      else{ return [];}})
      )
      .subscribe({
        next: (data) => {
          
          if (data.length < this.limit) {
            this.hasMorePosts = false; 
          } else {
            this.hasMorePosts = true; 
          }
  
          this.posts = [...this.posts, ...data]; 
          this.page++; 
  
          this.posts.forEach((post) => {
            this.postService.getMediaFromPost(post.id).subscribe({
              next: (media) => {
                post.mediaIds = media || null; 
              },
              error: (err) => console.log(err), 
            });
          });
  
          this.loading.set(false);
        },
        error: (err) => {
          console.error(err);
          this.loading.set(false); 
        },
      });
  }

  onScroll() {
    this.loadPostsTrigger.next();
  }

  emitChanges() {
    this.sortByChange.emit(this.sortBy);
    this.ageChange.emit(this.age);
  }
  
}
