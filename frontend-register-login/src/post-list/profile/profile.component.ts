import { Component, OnInit } from '@angular/core';
import { HistoryComponent } from "../history/history.component";
import { NgIf } from '@angular/common';
import { Post } from '../../interfaces/post';
import { Comment } from '../../interfaces/comment';
import { PostService } from '../../services/post.service';
import { NavigationEnd, Router } from '@angular/router';

@Component({
  selector: 'profile',
  imports: [NgIf],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {

  constructor(private postService:PostService,private route:Router)
  {}

  ngOnInit(): void {
    
    this.route.events.subscribe((event)=>{
      if(event instanceof NavigationEnd){
        var seg= this.route.url.split("/");

        this.user=seg[seg.length-1];
      }
    })
  }

  private user:string='';
  public loadNumber:number=-1;


  changeNumber(value:number){
    this.loadNumber=value;
  }

  public posts:Post[]=[];
  public comments:Comment[]=[];
  public upVoted:Post[]=[];
  public downVoted:Post[]=[];

  loadData(){
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
    
    this.postService.getPostsFromUser(this.user).subscribe({
      next:(data)=>{
        this.posts=data;
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
      },
      error:(err)=>{
        console.log(err);
      }
    });
  }

}
