import { Component, Input, OnInit } from '@angular/core';
import { Comment } from '../../../interfaces/comment';
import { CommonModule, NgIf } from '@angular/common';
import { format } from 'date-fns';
import { CommentService } from '../../../services/comment.service';
import { Router } from '@angular/router';
import { PostService } from '../../../services/post.service';

@Component({
  selector: 'comment-profile',
  imports: [NgIf,CommonModule],
  templateUrl: './comment-profile.component.html',
  styleUrl: './comment-profile.component.scss'
})
export class CommentProfileComponent implements OnInit{

  constructor(private commentService:CommentService,
    private route:Router,
    private postService:PostService,
  ){}

  @Input() comment= {} as Comment;

  public date:string='';
  public voted:boolean|null=null;
  private user:string='';

  ngOnInit(): void {
    this.date = format(new Date(this.comment.dateOfComment!),'dd/MM/yy HH:mm');
    //console.log(this.comment);

    this.commentService.getVoteValue(this.comment.id!,this.comment.username!).subscribe({
      next:(data)=>{
        this.voted=data;
      }
    })
  }


  voteOnComment(value:boolean){
    //this.voted=value;

    this.commentService.addCommentVote(this.comment.id!,this.comment.username!,value).subscribe({
      next:(data)=>{
        //var votee=JSON.parse(data) as boolean | null;
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

  routeToPost(){
    this.postService.getPost(this.comment.postId!).subscribe({
      next:(data)=>{
        this.route.navigateByUrl(`/mainPage/community/${data.communityName}/post/${data.id}`);
      },
      error:(err)=>{
        console.log(err);
      }
    });
    //this.route.navigateByUrl(`/mainPage/community/${this.comment.c}`)
  }
}
