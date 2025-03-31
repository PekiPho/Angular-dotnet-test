import { Component, Input, OnChanges, OnInit, signal, SimpleChanges } from '@angular/core';
import { Post, PostToSend } from '../../interfaces/post';
import { PostComponent } from '../post/post.component';
import { ActivatedRoute } from '@angular/router';
import { CommunityToSend } from '../../interfaces/community';
import { PostService } from '../../services/post.service';
import { NgFor, NgIf } from '@angular/common';
import { CommunityService } from '../../services/community.service';
import {InfiniteScrollDirective} from 'ngx-infinite-scroll';

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
  limit:number=50;
  page:number=1;
  
  loading=signal(false);
  @Input() sortBy:string="Hot";
  @Input() age:string="Today";

  constructor(private postService:PostService,private communityService:CommunityService){}

  ngOnInit(): void {
    this.communityService.communityy$.subscribe({
      next:(data)=>{
        if(data)
          this.community=data;
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{}
    }
      // if(community){
      //   this.community=community;
      //   this.getPostsBySort();
      // }
    )
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['age'] || changes['sortBy']){
      this.posts=[];
      this.page=1;
      this.hasMorePosts=true;
      this.getPostsBySort();
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

  getPostsBySort(){

    //console.log(this.sortBy + " " + this.age);


    if(!this.hasMorePosts || this.loading()){
      return;
    }
    this.loading.set(true);

    var comm=this.community.name;
    console.log(comm);
    if(comm==null){
      this.loading.set(false);
      return;
    }
      
    this.postService.getPostsBySort(comm,this.sortBy,this.page,this.limit,this.age).subscribe({
      next:(data)=>{
        if(data.length<this.limit)
          this.hasMorePosts=false;
        this.posts=[...this.posts,...data];
        this.page++;
        console.log(this.posts);
        console.log(this.loading());
      },
      error:(err)=>{
        console.log(err);
        this.loading.set(false);
      },
      complete:()=>{
        this.loading.set(false);
      }
    });
    
  }

  onScroll(){
    this.getPostsBySort();
  }
}
