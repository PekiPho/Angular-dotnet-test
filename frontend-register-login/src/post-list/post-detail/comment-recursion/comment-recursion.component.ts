import { Component, Input, input, NgModule, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Comment } from '../../../interfaces/comment';
import { NgFor, NgIf, NumberSymbol } from '@angular/common';
import {format} from 'date-fns';
import { CommentService } from '../../../services/comment.service';
import { UserServiceService } from '../../../services/user-service.service';
import { User } from '../../../interfaces/user';
import { PostService } from '../../../services/post.service';
import { Route, Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'comment-recursion',
  imports: [NgIf,NgFor,FormsModule],
  templateUrl: './comment-recursion.component.html',
  styleUrl: './comment-recursion.component.scss'
})
export class CommentRecursionComponent implements OnInit,OnChanges{

  constructor(private commentService:CommentService,
      private userService:UserServiceService,
      private postService:PostService,
      private route:Router,
  ){}

  @Input() comment:Comment={};
  @Input() level:number=-1;

  private user={} as User;

  public date:any;
  public voted:boolean | null = null;
  public visible:boolean=false;

  public showReply:boolean=false;
  public replyContent:string='';

  ngOnInit(): void {
    //console.log(this.comment.dateOfComment);
    this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');

    this.userService.userr$.subscribe({
      next:(data)=>{
        this.user= data as User;
      },
      error:(err)=>{
        console.log(err);
      }
    });


    this.commentService.getVoteValue(this.comment.id!,this.user.username).subscribe({
      next:(data)=>{
        this.voted=data;
      }
    })
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(changes['comment']){
      this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');
    }
  }

  voteOnComment(value:boolean){
    //this.voted=value;

    this.commentService.addCommentVote(this.comment.id!,this.user.username,value).subscribe({
      next:(data)=>{
        if(data===null){
          if(this.voted)
            this.comment.vote!--;
          else this.comment.vote!++;

          this.voted=null;
        }  
        else{
          if(this.voted===null){
           if(data)
            this.comment.vote!++;
          else this.comment.vote!--;
        }
        else{
          if(data)
            this.comment.vote!+=2;
          else this.comment.vote!-=2;
        }

        this.voted=data as boolean;
        }
    }});
  }

  toggleReply(){
    this.showReply=!this.showReply;

    if (!this.showReply) this.replyContent = '';
  }

  submitReply(){
    if(this.replyContent!=''){

      var seg=this.route.url.split('/');

    var postId=seg[seg.length-1];

    var comm={}as Comment;

    comm.content=this.replyContent;
    comm.username=this.user.username;
    comm.postId=postId;
    comm.replyToId=this.comment.id;

    this.commentService.addComment(this.user.username,postId,comm.replyToId!,comm).subscribe({
      next:(data)=>{
          location.reload();
      },
      error:(err)=>{
        console.log(err);
      }
    });
    }
    
  }

  getWidth(level:number){
    return `calc(100% - ${level*20}px)`;
  }
}
