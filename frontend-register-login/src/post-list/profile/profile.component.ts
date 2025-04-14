import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { HistoryComponent } from "../history/history.component";
import { NgFor, NgIf } from '@angular/common';
import { Post } from '../../interfaces/post';
import { Comment } from '../../interfaces/comment';
import { PostService } from '../../services/post.service';
import { NavigationEnd, Router } from '@angular/router';
import { PostComponent } from "../post/post.component";
import { Subscription } from 'rxjs';

@Component({
  selector: 'profile',
  imports: [NgIf, NgFor, PostComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit,OnDestroy {

  constructor(private postService:PostService,private route:Router)
  {}

  private routeSubs:Subscription= new Subscription();

  ngOnInit(): void {

    var seg=this.route.url.split('/');

    this.user=seg[seg.length-1];
    
    // this.routeSubs= this.route.events.subscribe((event)=>{
    //   if(event instanceof NavigationEnd){
    //     var seg= this.route.url.split("/");

    //     seg.forEach(s=>{
    //       console.log(s);
    //     })
    //     console.log(this.user);
    //   }
    // })

  }

  ngOnDestroy(): void {
    if(this.routeSubs)
      this.routeSubs.unsubscribe();
  }

  private user:string='';
  public loadNumber:number=-1;


  changeNumber(value:number){
    this.loadNumber=value;

    this.loadData();
  }

  public posts:Post[]=[];
  public comments:Comment[]=[];
  public upVoted:Post[]=[];
  public downVoted:Post[]=[];

  loadData(){

    console.log(this.user);

    if(this.loadNumber===0){
      this.loadPosts();
    }
    
    if(this.loadNumber===1){

    }

    if(this.loadNumber===2){
      this.loadUpVoted();
    }

    if(this.loadNumber===3){
      this.loadDownVoted();
    }
  }

  loadPosts(){

    this.comments=[];
    this.upVoted=[];
    this.downVoted=[];

    // need to make it to load 50 by 50
    // will do some other time
    console.log(this.user);
    this.postService.getPostsFromUser(this.user).subscribe({
      next:(data)=>{
        this.posts=data;
        this.loadMedia();
      },
      error:(err)=>{
        console.log(err);
      }
    });

  }

  loadComments(){

  }

  loadUpVoted(){

    this.posts=[];
    this.comments=[];
    this.downVoted=[];

    this.postService.getXVotedPostsFromUser(this.user,true).subscribe({
      next:(data)=>{
        this.upVoted=data;
        this.loadMedia();
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }

  loadDownVoted(){

    this.posts=[];
    this.comments=[];
    this.upVoted=[];

    this.postService.getXVotedPostsFromUser(this.user,false).subscribe({
      next:(data)=>{
        this.downVoted=data;
        //console.log(data);
        this.loadMedia();
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }

  loadMedia(){
    if(this.loadNumber===0){
      this.posts.forEach(post=>{
        this.postService.getMediaFromPost(post.id).subscribe({
          next:(data)=>{
            post.mediaIds=data;
          },
          error:(err)=>{
            console.log(err);
          }
        });
      })
    }

    if(this.loadNumber===2){
      this.upVoted.forEach(post=>{
        this.postService.getMediaFromPost(post.id).subscribe({
          next:(data)=>{
            post.mediaIds=data;
          },
          error:(err)=>{
            console.log(err);
          }
        });
      })
    }

    if(this.loadNumber===3){
      this.downVoted.forEach(post=>{
        this.postService.getMediaFromPost(post.id).subscribe({
          next:(data)=>{
            post.mediaIds=data;
          },
          error:(err)=>{
            console.log(err);
          }
        });
      })
    }
  }

}
