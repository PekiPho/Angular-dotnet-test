import { ChangeDetectorRef, Component, Input, OnInit } from '@angular/core';
import { Post, PostToSend, Vote } from '../../interfaces/post';
import { formatDate, NgClass, NgFor, NgIf } from '@angular/common';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { PostService } from '../../services/post.service';
import { UserServiceService } from '../../services/user-service.service';
import { User } from '../../interfaces/user';
import { Router } from '@angular/router';
import { CommentService } from '../../services/comment.service';

@Component({
  selector: 'post',
  imports: [NgIf,NgFor,NgClass],
  templateUrl: './post.component.html',
  styleUrl: './post.component.scss'
})
export class PostComponent implements OnInit{

  constructor(private sanitizer:DomSanitizer,
    private userService:UserServiceService,
    private postService:PostService,
    private router:Router,
    private commentService:CommentService,
  ){}

  @Input() public post= {} as Post;

  //public sanitizedUrl:SafeUrl | null = null;

  public isLink:boolean=false;

  private user:User | null = null;

  public voted:boolean|null=null;

  public copied:boolean=false;

  public date:any;


  public commCount:number=0; // to add comment count fetching

  ngOnInit(): void {
    console.log(this.post);

    this.userService.userr$.subscribe((user: User | null)=>{
      if(user){
        this.user=user;
      }
    });

    this.date = this.post.dateOfPost ? (new Date(this.post!.dateOfPost)).toLocaleDateString('en-GB') : null;

    this.postService.getCurrentVote(this.post,this.user!.username).subscribe({
      next:(data)=>{
        if(data!=null)
        {
          if(data)
            this.voted=true;
          else this.voted=false;
        }
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{}
    });

    //comm count fetch
    this.commentService.getCommentCount(this.post.id).subscribe({
      next:(data)=>{
        this.commCount=data;
      },
      error:(err)=>{
        console.log(err);
      }
    });

   //console.log(this.post);
    if(this.post.description==null){

    }
    else{
      if(this.checkURL(this.post.description)){
        //console.log("Is url!!");
        this.isLink=true;
        //console.log(this.post.mediaIds);
        //this.sanitizedUrl=this.sanitizer.bypassSecurityTrustResourceUrl(this.post.description);
      }
      else{

      }
    }
  }

  checkURL(name:string){
    const regex=/^(https?|ftp):\/\/[^\s/$.?#].[^\s]*$/i;

    return regex.test(name);
  }

  voteOnPost(votee:boolean){
    //console.log(votee);
    this.postService.addVote(this.post,this.user!.username,votee).subscribe({
      next:(data)=>{
        //console.log(data);
        //this.voted=votee;
        var vote=JSON.parse(data) as Vote;
        this.post.vote= vote.votes;
        
        //console.log(this.voted + " " +votee);
        if(this.voted===votee){
          this.voted=null;
          //console.log(this.voted);
        }
        else{
          this.voted=votee;
        }
        
      },
      error:(err)=>{
        console.log(err);
      },
      complete:()=>{}
    });
  }

  openBigPost(){
    this.router.navigateByUrl(`/mainPage/community/${this.post.communityName}/post/${this.post.id}`);
  }

  shareLink(){
    var base=window.location.origin;
    var url = this.router.url.split('/mainPage')[0]+"mainPage";

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

  isVideo(url:string){
    return /\.(mp4|mov|avi)$/i.test(url);
  }

  isImage(url:string){
    return /\.(jpg|jpeg|png)$/i.test(url);
  }

  navigate(){
    this.router.navigateByUrl(`/mainPage/community/${this.post.communityName}/post/${this.post.id}`);
  }

  navigateUser(){
    this.router.navigateByUrl(`/mainPage/profile/${this.post.username}`);
  }

  navigateCommunity(){
    this.router.navigateByUrl(`/mainPage/community/${this.post.communityName}`);
  }
}
