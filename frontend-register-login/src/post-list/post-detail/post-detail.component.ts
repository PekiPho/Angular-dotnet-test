import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { CommunityInfoComponent } from "../community-info/community-info.component";
import { PostService } from '../../services/post.service';
import { ActivatedRoute } from '@angular/router';
import { Post, Vote } from '../../interfaces/post';
import { NgIf } from '@angular/common';
import { Media } from '../../interfaces/media';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';

@Component({
  selector: 'post-detail',
  imports: [CommunityInfoComponent,NgIf],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.scss'
})
export class PostDetailComponent implements OnInit {

  constructor(private postService:PostService,
    private activatedRoute:ActivatedRoute,
    private userService:UserServiceService,
  ){}

  private user={} as User;
  private postId:string='';

  public cantLoad:boolean=false;
  public hasMedia:boolean=false;
  public hasVoted:boolean | null=null;
  public commCount:number=0;
  public post={}as Post;

  ngOnInit(): void {
    
    this.activatedRoute.url.subscribe(url=>{
      const urlPath=url.map(c=>c.path);
      this.postId=urlPath[3];
      //console.log(this.postId);
    });

    this.userService.userr$.subscribe({
      next:(data)=>{
        this.user= data ? data : {} as User;
      },
      error:(err)=>{
        console.log(err);
      }
    });



    if(this.postId!=''){
      this.postService.getPost(this.postId).subscribe({
        next:(data)=>{
          this.post=data;
          if(this.post.mediaIds!=null){
            this.hasMedia=true;
          }

          this.postService.getCurrentVote(this.post,this.user.username).subscribe({
            next:(data)=>{
              if(data!=null){
                if(data){
                  this.hasVoted=true;
                }
                else this.hasVoted=false;
              }

              console.log(this.hasVoted);
            },
            error:(err)=>{
              console.log(err);
            }
          })
          
          
          this.postService.getMediaFromPost(this.postId).subscribe({
            next:(data)=>{
              //console.log(data);
              if(data== null){
                this.hasMedia=false;
                this.post.mediaIds=null;
              }
              else{
                this.post.mediaIds=data as Media[];
              }
            },
            error:(err)=>{
              console.log(err);
            },
            complete:()=>{}
          });

          //this.postService.getCurrentVote(this.post,)
        },
        error:(err)=>{
          console.log(err);
        },
        complete:()=>{}
      });

      
      //console.log(this.hasMedia);
    }
    else{
      this.cantLoad=true;
    }
  }


  voteOnPost(value:boolean){
    this.postService.addVote(this.post,this.user.username,value).subscribe({
      next:(data)=>{
        var vote=JSON.parse(data) as Vote;
        this.post.vote=vote.votes;

        if(this.hasVoted===value)
          this.hasVoted=null;
        else{
          this.hasVoted=value;
        }
          
      }
    })
  }
}
