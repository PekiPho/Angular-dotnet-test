import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { CommunityInfoComponent } from "../community-info/community-info.component";
import { PostService } from '../../services/post.service';
import { ActivatedRoute, Router } from '@angular/router';
import { Post, Vote } from '../../interfaces/post';
import { NgFor, NgIf } from '@angular/common';
import { Media } from '../../interfaces/media';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';
import { CommentService } from '../../services/comment.service';
import { Comment } from '../../interfaces/comment';
import { CommentRecursionComponent } from "./comment-recursion/comment-recursion.component";

@Component({
  selector: 'post-detail',
  imports: [CommunityInfoComponent, NgIf, CommentRecursionComponent,NgFor],
  templateUrl: './post-detail.component.html',
  styleUrl: './post-detail.component.scss'
})
export class PostDetailComponent implements OnInit {

  constructor(private postService:PostService,
    private activatedRoute:ActivatedRoute,
    private userService:UserServiceService,
    private commentService:CommentService,
    private route:Router,
  ){}

  private user={} as User;
  private postId:string='';

  public cantLoad:boolean=false;
  public hasMedia:boolean=false;
  public hasVoted:boolean | null=null;
  public copied:boolean=false;
  public commCount:number=0;
  public post={}as Post;

  public comments:Comment[]=[];

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

              //console.log(this.hasVoted);
            },
            error:(err)=>{
              console.log(err);
            }
          });


          this.commentService.getCommentsFromPost(this.postId).subscribe({
            next:(data)=>{
              this.comments=[...data];

              this.commentDict={};

              this.comments.forEach(comment=>{
                var id=comment.id as string;
                this.commentDict[id]=comment;
              });
              //console.log(this.comments);

              
              this.root=[];
              this.buildTree();
            },
            error:(err)=>{
              console.log(err);
            }
          }); 
          
          this.commentService.getCommentCount(this.postId).subscribe({
            next:(data)=>{
              //console.log(data);
              this.commCount=data;
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

  addComment(){
    var content=(document.querySelector(".comment-send") as HTMLTextAreaElement).value;

    //console.log(comment);
    var comm={}as Comment;

    comm.content=content;
    comm.username=this.user.username;
    comm.postId=this.postId;
    comm.replyToId="null";

    this.commentService.addComment(this.user.username,this.postId,"null",comm).subscribe({
      next:(data)=>{
        comm=data;
        this.comments=[comm ,...this.comments];
        (document.querySelector(".comment-send") as HTMLTextAreaElement).value='';

        this.commentDict={};
        this.root=[];
        this.buildTree();
        //console.log(content);
      },
      error:(err)=>{
        console.log(err);
      }
    })
  }

  public commentDict:{[id:string]:Comment}={};
  public root:Comment[]=[];

  buildTree(){
    this.root=[];

  Object.values(this.commentDict).forEach(comment => {
    comment.replies = [];
  });
    

  //console.log(this.comments);
   Object.values(this.commentDict).forEach(comment => {
      if(comment.replyToId){
        const parent=this.commentDict[comment.replyToId];

        if(parent){
          parent.replies=parent.replies || [];
          parent.replies.push(comment);
        }
      }
      else{
        this.root.push(comment);
      }
    });

    //console.log(this.root);
  }


  shareLink(){
    var base=window.location.origin;
    var url = this.route.url.split('/mainPage')[0]+"mainPage";

    var postUrl=`${base}/${url}/community/${this.post.communityName}/post/${this.post.id}`;

    navigator.clipboard.writeText(postUrl).then(
      ()=>{
        this.copied=true;
        setTimeout(()=>{
          this.copied=false;
        },3000);
      },
      (err)=>{
        console.log(err);
      }
    );
  }

}
