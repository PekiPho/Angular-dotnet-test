import { Component, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { HistoryComponent } from "../history/history.component";
import { NgFor, NgIf } from '@angular/common';
import { Post } from '../../interfaces/post';
import { Comment } from '../../interfaces/comment';
import { PostService } from '../../services/post.service';
import { NavigationEnd, Router } from '@angular/router';
import { PostComponent } from "../post/post.component";
import { Subscription } from 'rxjs';
import { CommentService } from '../../services/comment.service';
import { CommentProfileComponent } from './comment-profile/comment-profile.component';

@Component({
  selector: 'profile',
  imports: [NgIf, NgFor, PostComponent,CommentProfileComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit,OnDestroy {

  constructor(private postService:PostService,
    private route:Router,
    private commentService:CommentService
  ){}

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
    this.loadData();

  }

  ngOnDestroy(): void {
    if(this.routeSubs)
      this.routeSubs.unsubscribe();
  }

  private user:string='';
  public loadNumber:number=0;
  public page:number=1;
  public isLoading:boolean=false;
  public hasMore:boolean=true;


  changeNumber(value:number){
    this.loadNumber=value;
    this.page=1;
    this.hasMore=true;

    this.loadData();
  }

  public posts:Post[]=[];
  public comments:Comment[]=[];
  public upVoted:Post[]=[];
  public downVoted:Post[]=[];

  loadData(){

    //console.log(this.user);

    if(this.loadNumber===0){
      this.loadPosts();
    }
    
    if(this.loadNumber===1){
      this.loadComments();
    }

    if(this.loadNumber===2){
      this.loadUpVoted();
    }

    if(this.loadNumber===3){
      this.loadDownVoted();
    }
  }

  loadPosts(){

    if(this.isLoading || !this.hasMore) return;

    this.isLoading=true;

    if(this.page===1){
      this.comments=[];
      this.upVoted=[];
      this.downVoted=[];
      this.posts=[];
    }

    

    // need to make it to load 50 by 50
    // will do some other time
    //console.log(this.user);
    this.postService.getPostsFromUser(this.user,this.page).subscribe({
      next:(data)=>{
        if(data.length<50){
          this.hasMore=false;
        }

        this.posts=[...this.posts,...data];
        this.loadMedia();
        this.page++;
        this.isLoading=false;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
      }
    });

  }

  loadComments(){
    this.posts=[];
    this.upVoted=[];
    this.downVoted=[];


    this.commentService.getCommentsFromUser(this.user).subscribe({
      next:(data)=>{
        //console.log(data);
        this.comments=data;
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }


  loadUpVoted(){

    if(this.isLoading || !this.hasMore) return;

    this.isLoading=true;

    if(this.page===1){
      this.posts=[];
      this.comments=[];
      this.downVoted=[];
      this.upVoted=[];
    }

    this.postService.getXVotedPostsFromUser(this.user,true,this.page).subscribe({
      next:(data)=>{
        if(data.length<50){
          this.hasMore=false;
        }

        this.upVoted=[...this.upVoted,...data];
        this.loadMedia();
        this.page++;
        this.isLoading=false;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
      }
    });
  }

  loadDownVoted(){

    if(this.isLoading || !this.hasMore) return;

    this.isLoading=true;

    if(this.page===1){
      this.posts=[];
      this.comments=[];
      this.upVoted=[];
      this.downVoted=[];
    }
    

    this.postService.getXVotedPostsFromUser(this.user,false,this.page).subscribe({
      next:(data)=>{
        if(data.length<50){
          this.hasMore=false;
        }

        this.downVoted=[...this.downVoted,...data];
        this.page++;
        //console.log(data);
        this.loadMedia();

        this.isLoading=false;
      },
      error:(err)=>{
        console.log(err);
        this.isLoading=false;
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

  onScroll(event:Event){
    const element = event.target as HTMLElement;
    const threshold = 150;

    const scrollTop = element.scrollTop;
    const clientHeight = element.clientHeight;
    const scrollHeight = element.scrollHeight;

    if (scrollTop + clientHeight >= scrollHeight - threshold && !this.isLoading && this.hasMore) {
      //this.page++;
      this.loadData();
    }
  }

}
